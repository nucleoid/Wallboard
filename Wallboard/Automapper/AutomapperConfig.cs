using System.ServiceModel.Syndication;
using AutoMapper;
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
                map.CreateMap<SyndicationItem, BuildStatusModel>()
                 .ForMember(d => d.BuildStatus, o => o.MapFrom(s => s))
                 .ForMember(d => d.StatusClass, o => o.MapFrom(s => s.Title.Text.ToLowerInvariant().Contains("failed") ? "failure" : "success"));
            });
        }
    }
}