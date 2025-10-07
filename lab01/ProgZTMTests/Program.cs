using System;
using JP_NET.Lab1;
using Xunit;

namespace JP_NET.Lab1.Tests
{
    public class JSONProcessorTests
    {
        string sampleJson = @"
            {
                ""lastUpdate"": ""2025-10-07T10:00:00Z"",
                ""departures"": [
                    {
                        ""id"": ""1"",
                        ""estimatedTime"": ""2025-10-07T10:05:00Z"",
                        ""headsign"": ""Dworzec Główny"",
                        ""routeShortName"": ""158"",
                        ""routeId"": 158,
                        ""tripId"": 42,
                        ""status"": ""REALTIME"",
                        ""timestamp"": ""2025-10-07T09:59:00Z"",
                        ""trip"": 42
                    }
                ]
            }";

        [Fact]
        public void ProcessJSON_CheckDeserialisationOfNotNullJSON()
        {
            var processor = new JSONProcessor();
            Timetable[] result = processor.ProcessJSON(sampleJson);

            Assert.NotNull(result);
            Assert.Single(result);

        }

        [Fact]
        public void ProcessJSON_CheckFirstElementOfJSONList()
        {
            var processor = new JSONProcessor();
            Timetable[] result = processor.ProcessJSON(sampleJson);
            var timetable = result[0];

            Assert.NotNull(timetable.departures);
            Assert.Single(timetable.departures);

        }

        [Fact]
        public void ProcessJSON_ChangeDatetimeFromUTCToLocal()
        {
            var processor = new JSONProcessor();
            Timetable[] result = processor.ProcessJSON(sampleJson);
            var timetable = result[0];

            var expectedLocal = DateTime.Parse("2025-10-07T10:00:00Z").ToLocalTime();
            Assert.Equal(expectedLocal, timetable.lastUpdate);

            var expectedLocalOnDetail = DateTime.Parse("2025-10-07T10:05:00Z").ToLocalTime();
            Assert.Equal(expectedLocalOnDetail, timetable.departures[0].estimatedTime);

        }

        [Fact]
        public void ProcessJSON_VerifyHeadsignContent()
        {
            var processor = new JSONProcessor();
            Timetable[] result = processor.ProcessJSON(sampleJson);
            var headsign = result[0].departures[0].headsign;

            Assert.Equal("Dworzec Główny", headsign);
        }

    }
}
