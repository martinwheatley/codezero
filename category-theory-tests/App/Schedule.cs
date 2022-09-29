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

internal class Shift
{
    public Shift(EmployeeType[] requiredEmployeeTypes, BoundedInterval<DateTime> interval)
    {
        Interval = interval;
        RequiredEmployeeTypes = requiredEmployeeTypes;
    }

    public EmployeeType[] RequiredEmployeeTypes { get; }
    public BoundedInterval<DateTime> Interval { get; }

    public override string ToString() => Interval.ToString();
}

internal enum EmployeeType
{
    T1,
    T2
}

internal record Employee
{
    public Employee(int id, string navn, EmployeeType type)
    {
        Id = id;
        Navn = navn;
        Type = type;
    }

    public string Navn { get; }
    public int Id { get; }
    public EmployeeType Type { get; }
}

internal class ShiftConfiguration
{
    private readonly BoundedInterval<DateTime>[] _intervals;
    private readonly Func<BoundedInterval<DateTime>, bool>[] _predicates;

    public static ShiftConfiguration Empty =>
        new(Array.Empty<BoundedInterval<DateTime>>(),
            Array.Empty<Func<BoundedInterval<DateTime>, bool>>());

    public ShiftConfiguration(BoundedInterval<DateTime>[] intervals, Func<BoundedInterval<DateTime>, bool>[] intervalPredicates)
    {
        _intervals = intervals;
        _predicates = intervalPredicates;
    }

    public bool Contains(Shift shift)
    {
        var conflictsWithReservedDates = _intervals.Any(r => r.Overlaps(shift.Interval));
        var conflictsWithReservedRules = _predicates.Any(f => f(shift.Interval));

        return conflictsWithReservedDates || conflictsWithReservedRules;
    }
}

internal class EmployeeShiftConfiguration
{
    private readonly ShiftConfiguration _reservedIntervals;
    private readonly ShiftConfiguration _preferredIntervals;

    public EmployeeShiftConfiguration(
        Employee employee,
        ShiftConfiguration reservedIntervals,
        ShiftConfiguration preferredIntervals)
    {
        Employee = employee;
        _reservedIntervals = reservedIntervals;
        _preferredIntervals = preferredIntervals;
    }

    public Employee Employee { get; }

    public bool CanWork(Shift shift) =>
        shift.RequiredEmployeeTypes.Contains(Employee.Type) &&
        !_reservedIntervals.Contains(shift);

    public bool Prefers(Shift shift) =>
        _preferredIntervals.Contains(shift);

    public override string ToString() => Employee.ToString();
}

public class Application
{
    private static EmployeeShiftConfiguration Hans
    {
        get
        {
            var employee = new Employee(1, "Hans", EmployeeType.T1);

            var preferredPredicates =
                new Func<BoundedInterval<DateTime>, bool>[1]
                {
                    i => i.From.Value.Hour is 8 && i.To.Value.Hour is 16
                };

            var reservedPredicates =
                new Func<BoundedInterval<DateTime>, bool>[2]
                {
                    i => i.From.Value.DayOfWeek is DayOfWeek.Friday,
                    i => i.From.Value.DayOfWeek is DayOfWeek.Thursday && TimeOnly.FromDateTime(i.To.Value).Hour is > 15,
                };

            var preferredShifts = new ShiftConfiguration(Array.Empty<BoundedInterval<DateTime>>(), preferredPredicates);
            var reservedShifts = new ShiftConfiguration(Array.Empty<BoundedInterval<DateTime>>(), reservedPredicates);

            return new EmployeeShiftConfiguration(employee, reservedShifts, preferredShifts);
        }
    }

    private static EmployeeShiftConfiguration Grethe
    {
        get
        {
            var employee = new Employee(2, "Grethe", EmployeeType.T2);

            var dob = new DateOnly(2022, 1, 24);

            var reservedPredicates =
                new Func<BoundedInterval<DateTime>, bool>[1]
                {
                    i => i.From.Value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
                };

            var reservedForDob =
                new BoundedInterval<DateTime>[1]
                {
                    new BoundedInterval<DateTime>(
                        Bound<DateTime>.Inclusive(dob.ToDateTime(default)),
                        Bound<DateTime>.Exclusive(dob.AddDays(1).ToDateTime(default)))
                };

            var reservedShifts = new ShiftConfiguration(reservedForDob, reservedPredicates);

            return new EmployeeShiftConfiguration(employee, reservedShifts, ShiftConfiguration.Empty);
        }
    }

    private static EmployeeShiftConfiguration HrAndersen
    {
        get
        {
            var employee = new Employee(3, "Hr. Andersen", EmployeeType.T2);

            var reservedPredicates =
                new Func<BoundedInterval<DateTime>, bool>[1]
                {
                    i => i.From.Value.DayOfWeek is DayOfWeek.Monday
                };

            var preferredPredicates =
                new Func<BoundedInterval<DateTime>, bool>[1]
                {
                    i => i.From.Value.DayOfWeek is DayOfWeek.Tuesday
                };

            var preferredShifts =
                new ShiftConfiguration(
                    Array.Empty<BoundedInterval<DateTime>>(),
                    preferredPredicates);

            var reservedShifts =
                new ShiftConfiguration(
                    Array.Empty<BoundedInterval<DateTime>>(),
                    reservedPredicates);

            return new EmployeeShiftConfiguration(employee, reservedShifts, preferredShifts);
        }
    }

    [Fact]
    public void Test()
    {
        // Schedule:
        //  Start:  2022-01-01 08:00
        //  End:    2022-02-01 08:00

        var emp1 = new Employee(1, "Hans", EmployeeType.T2);
        var emp2 = new Employee(2, "Grethe", EmployeeType.T1);

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
                    var isWeekend = day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

                    var date = DateOnly.FromDateTime(day);
                    var startTime = day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
                        ? new TimeOnly(10, 0)
                        : new TimeOnly(8, 0);

                    var endTime = new TimeOnly(16, 0);

                    var interval =
                        new BoundedInterval<DateTime>(
                            Bound<DateTime>.Inclusive(date.ToDateTime(startTime)),
                            Bound<DateTime>.Exclusive(date.ToDateTime(endTime)));

                    var requiredEmplType =
                        isWeekend
                            ? new EmployeeType[1] { EmployeeType.T1 }
                            : new EmployeeType[1] { EmployeeType.T2 };

                    var shift = new Shift(requiredEmplType, interval);

                    return shift;
                })
                .ToArray();

        var employees =
            new EmployeeShiftConfiguration[3]
            {
                Hans,
                Grethe,
                HrAndersen
            };

        // Behov for at en employee skal være af en særlig type for at kunne dække en vagt.
        // Behov for en type, der holder nedenstående data, samt information om, hvilke vagter de enkelte employees er blevet tildelt.
        var result =
            shifts.ToDictionary(
                s => s,
                s => employees.Where(e => e.CanWork(s)).Select(e => (e, e.Prefers(s))).ToArray());
    }
}