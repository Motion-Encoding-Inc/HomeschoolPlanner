namespace HomeschoolPlanner.Domain

type DayMask = int // bitmask Mon..Sun

type ResourceType = | Book | Time | Custom

type PlanStrategy = | Push | CatchUp | Smart

type UnitIndex = int

type Planned =
    | BookUnits of UnitIndex list
    | Minutes of int

type Occurrence =
    { Date : System.DateOnly
      Planned : Planned }

type SchedulePreview =
    { Start : System.DateOnly
      End   : System.DateOnly
      Items : Occurrence list }

//module Scheduler =
//    val materialize :
//        strategy:PlanStrategy ->
//        allowedDays:DayMask ->
//        startDate:System.DateOnly ->
//        endDate:System.DateOnly ->
//        units:int option ->
//        minutesPerDay:int option ->
//        lookahead:int ->
//        SchedulePreview

//    val applySkip :
//        strategy:PlanStrategy ->
//        skipDate:System.DateOnly ->
//        current:SchedulePreview ->
//        SchedulePreview

//    val applyComplete :
//        current:SchedulePreview ->
//        date:System.DateOnly ->
//        unitOrMinutes:int ->
//        SchedulePreview
