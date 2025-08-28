namespace HomeschoolPlanner.Domain

open System

module DayMask =
    let ofDays (days: DayOfWeek list) =
        days
        |> List.fold (fun acc d -> acc ||| (1 <<< int d)) 0
    let contains (mask: DayMask) (day: DayOfWeek) =
        (mask &&& (1 <<< int day)) <> 0

module Scheduler =

    val materialize :
        strategy:PlanStrategy ->
        allowedDays:DayMask ->
        startDate:DateOnly ->
        endDate:DateOnly ->
        units:int option ->             // for Book/Custom
        minutesPerDay:int option ->     // for Time
        lookahead:int ->
        SchedulePreview

    val applySkip :
        strategy:PlanStrategy ->
        skipDate:DateOnly ->
        current:SchedulePreview ->
        SchedulePreview

    val applyComplete :
        current:SchedulePreview ->
        date:DateOnly ->
        unitOrMinutes:int ->
        SchedulePreview

// Temporary stub implementations so API compiles immediately
module SchedulerImpl =
    open Scheduler

    let private empty start finish =
        { Start = start; End = finish; Items = [] }

    let materialize
        (strategy:PlanStrategy)
        (allowedDays:DayMask)
        (startDate:DateOnly)
        (endDate:DateOnly)
        (units:int option)
        (minutesPerDay:int option)
        (lookahead:int) : SchedulePreview =
        empty startDate endDate

    let applySkip _strategy _skipDate current = current
    let applyComplete current _date _amt      = current
