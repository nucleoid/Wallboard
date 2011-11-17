using AutoMapper;
using DDay.iCal;
using Wallboard.Models;

namespace Wallboard.Automapper
{
    public static class AutomapperConfig
    {
        public static void Setup()
        {
            Mapper.Reset();

            Mapper.Initialize(map =>
            {
                map.CreateMap<string, BuildStatusModel>()
                 .ForMember(d => d.BuildStatus, o => o.MapFrom(s => s))
                 .ForMember(d => d.StatusClass, o => o.MapFrom(s => s.ToLowerInvariant().Contains("failed") ? "failure" : "success"));
                map.CreateMap<IEvent, EventModel>()
                    .ForMember(d => d.Event, o => o.MapFrom(s => s))
                    .ForMember(d => d.Duration, o => o.MapFrom(s => s.IsAllDay ? "ALL DAY" : Duration(s)));
            });
        }

        private static string Duration(IEvent ievent)
        {
            return string.Format("{0} - {1}", ievent.Start.ToString("hh:mmtt"), ievent.End.ToString("hh:mmtt"));
        }
    }
}
