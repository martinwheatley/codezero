using category_theory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace category_theory_tests.App;

internal class Schedule
{
}

internal class Employee
{
    private readonly BoundedInterval<DateTime>[] _reservedIntervals;
    private readonly Func<BoundedInterval<DateTime>, bool>[] _reservationPredicates;

    public Employee(
        string name,
        BoundedInterval<DateTime>[] reservedIntervals,
        Func<BoundedInterval<DateTime>, bool>[] reservationPredicates)
    {
        Name = name;
        _reservedIntervals = reservedIntervals;
        _reservationPredicates = reservationPredicates;
    }

    public string Name { get; }

    public bool CanWork(BoundedInterval<DateTime> interval)
    {
        var conflictsWithReservedDates = _reservedIntervals.Any(r => r.Overlaps(interval));
        var conflictsWithReservedRules = _reservationPredicates.Any(f => f(interval));

        return !(conflictsWithReservedDates || conflictsWithReservedRules);
    }

    public override string ToString() => Name;
}

public class Application
{
    [Fact]
    public void Test()
    {
        // Schedule:
        //  Start:  2022-01-01 08:00
        //  End:    2022-02-01 08:00

        var scheduleStart = new DateTime(2022, 1, 1, 8, 0, 0);
        var scheduleEnd = new DateTime(2022, 2, 1, 8, 0, 0);

        var schedule =
            new BoundedInterval<DateTime>(
                Bound<DateTime>.Inclusive(scheduleStart),
                Bound<DateTime>.Exclusive(scheduleEnd));

        var shifts =
            Enumerable.Range(0, scheduleEnd.DayOfYear - scheduleStart.DayOfYear)
                .Select(i =>
                {
                    var day = scheduleStart.AddDays(i);
                    var date = DateOnly.FromDateTime(day);
                    var startTime = day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
                        ? new TimeOnly(10, 0)
                        : new TimeOnly(8, 0);

                    var endTime = new TimeOnly(16, 0);

                    var interval =
                        new BoundedInterval<DateTime>(
                            Bound<DateTime>.Inclusive(date.ToDateTime(startTime)),
                            Bound<DateTime>.Exclusive(date.ToDateTime(endTime)));

                    return interval;
                })
                .ToArray();

        var reservedPredicatesEmp1 =
            new Func<BoundedInterval<DateTime>, bool>[2]
            {
                i => i.From.Value.DayOfWeek is DayOfWeek.Friday, // hungover on fridays
                i => i.From.Value.DayOfWeek is DayOfWeek.Thursday && TimeOnly.FromDateTime(i.To.Value).Hour is > 15, // Sleeps late
            };

        var reservedPredicatesEmp2 =
            new Func<BoundedInterval<DateTime>, bool>[1]
            {
                i => i.From.Value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday, // no work on weekends
            };

        var employee2Dob = new DateOnly(2022, 1, 24);

        var employee1 =
            new Employee(
                "Grethe",
                Array.Empty<BoundedInterval<DateTime>>(),
                reservedPredicatesEmp1);

        var employee2 =
            new Employee(
                "Hans",
                new BoundedInterval<DateTime>[1]
                {
                    new BoundedInterval<DateTime>(
                        Bound<DateTime>.Inclusive(employee2Dob.ToDateTime(default)),
                        Bound<DateTime>.Exclusive(employee2Dob.AddDays(1).ToDateTime(default)))
                },
                reservedPredicatesEmp2);

        var employees =
            new Employee[2]
            {
                employee1,
                employee2
            };

        var result =
            shifts.ToDictionary(
                s => s,
                s => employees.Where(e => e.CanWork(s)).ToArray());
    }
}