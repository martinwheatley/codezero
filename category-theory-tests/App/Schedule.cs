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

internal record Activity
{
    public Activity(int id, ActivityType type, IReadOnlyDictionary<ResourceType, int> requiredResources, BoundedInterval<DateTime> timeSlot)
    {
        Id = id;
        Type = type;
        TimeSlot = timeSlot;
        RequiredResources = requiredResources;
    }

    public int Id { get; }
    public ActivityType Type { get; }
    public BoundedInterval<DateTime> TimeSlot { get; }
    public IReadOnlyDictionary<ResourceType, int> RequiredResources { get; } // Lav en mere deklarativ type; ActivityRequirements e.l.
    public bool CrossesDates =>
        DateOnly.FromDateTime(TimeSlot.From.Value).DayNumber !=
        DateOnly.FromDateTime(TimeSlot.To.Value).DayNumber;

    public override string ToString() => TimeSlot.ToString();
}

internal enum ActivityType
{
    A1,
    A2
}

internal enum ResourceType
{
    R1,
    R2
}

internal record Resource
{
    public Resource(int id, string name, ResourceType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public int Id { get; }
    public string Name { get; }
    public ResourceType Type { get; }
}

internal class ActivityConfiguration
{
    private readonly BoundedInterval<DateTime>[] _timeSlots;
    private readonly Func<BoundedInterval<DateTime>, bool>[] _predicates;

    public static ActivityConfiguration Empty =>
        new(Array.Empty<BoundedInterval<DateTime>>(),
            Array.Empty<Func<BoundedInterval<DateTime>, bool>>());

    public ActivityConfiguration(BoundedInterval<DateTime>[] timeSlots, Func<BoundedInterval<DateTime>, bool>[] timeSlotPredicates)
    {
        _timeSlots = timeSlots;
        _predicates = timeSlotPredicates;
    }

    public bool Contains(Activity activity)
    {
        var conflictsWithReservedDates = _timeSlots.Any(r => r.Overlaps(activity.TimeSlot));
        var conflictsWithReservedRules = _predicates.Any(f => f(activity.TimeSlot));

        return conflictsWithReservedDates || conflictsWithReservedRules;
    }
}

internal class ResourceActivityConfiguration
{
    private readonly ActivityConfiguration _reservedTimeSlots;
    private readonly ActivityConfiguration _preferredTimeSlots;

    public ResourceActivityConfiguration(
        Resource resource,
        ActivityConfiguration reservedTimeSlots,
        ActivityConfiguration prioritizedTimeSlots)
    {
        Resource = resource;
        _reservedTimeSlots = reservedTimeSlots;
        _preferredTimeSlots = prioritizedTimeSlots;
    }

    public Resource Resource { get; }

    public bool CanProcess(Activity activity) =>
        activity.RequiredResources.Keys.Contains(Resource.Type) &&
        !_reservedTimeSlots.Contains(activity);

    public bool Prefers(Activity activity) =>
        _preferredTimeSlots.Contains(activity);

    public override string ToString() => Resource.ToString();
}

public class Application
{
    private static ResourceActivityConfiguration Hans
    {
        get
        {
            var resource = new Resource(1, "Hans", ResourceType.R1);

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

            var preferredActivities = new ActivityConfiguration(Array.Empty<BoundedInterval<DateTime>>(), preferredPredicates);
            var reservedActivities = new ActivityConfiguration(Array.Empty<BoundedInterval<DateTime>>(), reservedPredicates);

            return new ResourceActivityConfiguration(resource, reservedActivities, preferredActivities);
        }
    }

    private static ResourceActivityConfiguration Grethe
    {
        get
        {
            var resource = new Resource(2, "Grethe", ResourceType.R2);

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

            var reservedActivities = new ActivityConfiguration(reservedForDob, reservedPredicates);

            return new ResourceActivityConfiguration(resource, reservedActivities, ActivityConfiguration.Empty);
        }
    }

    private static ResourceActivityConfiguration BroderGrimm
    {
        get
        {
            var resource = new Resource(3, "Hr. Andersen", ResourceType.R2);

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

            var preferredActivities =
                new ActivityConfiguration(
                    Array.Empty<BoundedInterval<DateTime>>(),
                    preferredPredicates);

            var reservedActivities =
                new ActivityConfiguration(
                    Array.Empty<BoundedInterval<DateTime>>(),
                    reservedPredicates);

            return new ResourceActivityConfiguration(resource, reservedActivities, preferredActivities);
        }
    }

    [Fact]
    public void Test()
    {
        var scheduleStart = new DateOnly(2022, 1, 1);
        var scheduleEnd = new DateOnly(2022, 2, 1);

        var activities =
            Enumerable.Range(0, scheduleEnd.DayNumber - scheduleStart.DayNumber - 1)
                .Select(i =>
                {
                    var date = scheduleStart.AddDays(i);
                    var isWeekend = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

                    var startTime = isWeekend
                        ? new TimeOnly(10, 0)
                        : new TimeOnly(8, 0);

                    var endTime = new TimeOnly(16, 0);

                    var interval =
                        new BoundedInterval<DateTime>(
                            Bound<DateTime>.Inclusive(date.ToDateTime(startTime)),
                            Bound<DateTime>.Exclusive(date.ToDateTime(endTime)));

                    var requiredResources =
                        isWeekend
                            ? new Dictionary<ResourceType, int>
                                {
                                    { ResourceType.R1, 1 }
                                }
                            : new Dictionary<ResourceType, int>
                                {
                                    { ResourceType.R2, 1 }
                                };
                    var type = isWeekend ? ActivityType.A1 : ActivityType.A2;

                    return new Activity(i, type, requiredResources, interval);
                })
                .ToArray();

        var resources =
            new ResourceActivityConfiguration[3]
            {
                Hans,
                Grethe,
                BroderGrimm
            };

        // Behov for at en employee skal være af en særlig type for at kunne dække en vagt.
        // Behov for en type, der holder nedenstående data, samt information om, hvilke vagter de enkelte employees er blevet tildelt.
        var result =
            activities.ToDictionary(
                activity => activity,
                activity => resources.Where(e => e.CanProcess(activity)).Select(e => (e, e.Prefers(activity))).ToArray());

        // HARD CONTRAINTS:
        //  1) En resource må ikke påtage sig to aktiviteter lige efter hinanden.
        //  2) En resource må ikke påtage sig aktiviteter på sine reserverede timeslots.
        //  3) En resource må ikke skal have fri et døgn efter endt aktivitet, hvis aktivitetens D(start) < D(slut).
        //  4) Minimér antal aktiviteter uden resourcer.

        var constraints =
            new Func<ResourceAllocation, bool>[1]
            {
                ra => !ra.HasOverlappingActivities()
            };

        var constraintStore = new ConstraintStore(constraints);

        // SOFT CONSTRAINTS:
        //  1) Minimér antal overarbejdstimer i perioden.
        //  2) En resource skal arbejde minimum X timer i perioden.

        // var scheduler = new Scheduler(algorithm);
        // var schedule = scheduler.Map(activities, resources);
    }
}

internal record ResourceAllocation
{
    public ResourceAllocation(Resource resource, List<Activity> allocatedActivities)
    {
        Resource = resource;
        AllocatedActivities = allocatedActivities;
    }

    public Resource Resource { get; }
    public List<Activity> AllocatedActivities { get; }

    public bool HasOverlappingActivities()
    {
        foreach (var activity in AllocatedActivities)
            if (AllocatedActivities.Any(a => a.Id != activity.Id && a.TimeSlot.Overlaps(activity.TimeSlot)))
                return true;

        return false;
    }
}

internal class ConstraintStore
{
    private readonly Func<ResourceAllocation, bool>[] _resourceAllocationConstraints;

    public ConstraintStore(Func<ResourceAllocation, bool>[] resourceAllocationConstraints)
    {
        _resourceAllocationConstraints = resourceAllocationConstraints;
    }
}