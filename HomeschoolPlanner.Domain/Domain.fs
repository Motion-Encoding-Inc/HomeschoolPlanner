namespace HomeschoolPlanner.Domain

/// Represents allowed days as a bitmask from Monday to Sunday.
type DayMask = int

/// Supported resource categories.
type ResourceType =
    | Book
    | Time
    | Custom

/// Strategies for planning how work is scheduled.
type PlanStrategy =
    | Push
    | CatchUp
    | Smart

/// 0-based index for book units.
type UnitIndex = int

/// What was planned for a given occurrence.
type Planned =
    | BookUnits of UnitIndex list
    | Minutes of int

/// A scheduled entry.
type Occurrence =
    { Date: System.DateOnly
      Planned: Planned }

/// A preview of the generated schedule.
type SchedulePreview =
    { Start: System.DateOnly
      End: System.DateOnly
      Items: Occurrence list }
