namespace HomeschoolPlanner.Domain

open System

module DayMaskOps =
    /// Build a DayMask from a list of allowed days.
    let ofDays (days: DayOfWeek list) =
        days |> List.fold (fun acc d -> acc ||| (1 <<< int d)) 0
    
    /// Check whether a mask contains a day.
    let contains (mask: DayMask) (day: DayOfWeek) =
        (mask &&& (1 <<< int day)) <> 0

module Scheduler =
    /// Internal helper to create an empty schedule preview.
    let private empty start finish =
        { Start = start; End = finish; Items = [] }

    /// Generate a preview schedule with no items.
    let materialize
        (strategy: PlanStrategy)
        (allowedDays: DayMask)
        (startDate: DateOnly)
        (endDate: DateOnly)
        (units: int option)
        (minutesPerDay: int option)
        (lookahead: int)
        : SchedulePreview =
        empty startDate endDate

    /// Ignore skips and return the current preview unchanged.
    let applySkip
        (_strategy: PlanStrategy)
        (_skipDate: DateOnly)
        (current: SchedulePreview)
        : SchedulePreview =
        current

    /// Ignore completions and return the current preview unchanged.
    let applyComplete
        (current: SchedulePreview)
        (_date: DateOnly)
        (_unitOrMinutes: int)
        : SchedulePreview =
        current
