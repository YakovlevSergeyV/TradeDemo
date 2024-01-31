namespace Infrastructure.Common.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Common.Convert;
    using Newtonsoft.Json;

    public class TimeFrameListConverter : JsonConverter<IEnumerable<TimeFrame>>
    {
        public override void WriteJson(JsonWriter writer, IEnumerable<TimeFrame> value, JsonSerializer serializer)
        {
            writer.WriteValue(string.Join(",", value.Select(x => x.Name).ToList()));
        }

        public override IEnumerable<TimeFrame> ReadJson(JsonReader reader, Type objectType,
            IEnumerable<TimeFrame> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return string.IsNullOrEmpty(name)
                ? new List<TimeFrame>()
                : name.Split(',').Select(TimeFrame.FromName).ToList();
        }
    }

    public class TimeFrameConverter : JsonConverter<TimeFrame>
    {
        public override void WriteJson(JsonWriter writer, TimeFrame value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override TimeFrame ReadJson(JsonReader reader, Type objectType, TimeFrame existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var name = (string)reader.Value;

            return name == null ? null : TimeFrame.FromName(name);
        }
    }

    [JsonConverter(typeof(TimeFrameConverter))]
    public class TimeFrame : Enumeration
    {
        public static TimeFrame Sec1 = new TimeFrame(1, "Sec1");
        public static TimeFrame Min1 = new TimeFrame(60, "Min1");
        public static TimeFrame Min3 = new TimeFrame(180, "Min3");
        public static TimeFrame Min5 = new TimeFrame(300, "Min5");
        public static TimeFrame Min15 = new TimeFrame(900, "Min15");
        public static TimeFrame Min30 = new TimeFrame(1800, "Min30");
        public static TimeFrame Hour1 = new TimeFrame(3600, "Hour1");
        public static TimeFrame Hour2 = new TimeFrame(7200, "Hour2");
        public static TimeFrame Hour3 = new TimeFrame(10800, "Hour3");
        public static TimeFrame Hour4 = new TimeFrame(14400, "Hour4");
        public static TimeFrame Hour6 = new TimeFrame(21600, "Hour6");
        public static TimeFrame Hour12 = new TimeFrame(43200, "Hour12");
        public static TimeFrame Day = new TimeFrame(86400, "Day");
        public static TimeFrame Week = new TimeFrame(604800, "Week");
        public static TimeFrame Month = new TimeFrame(2149200, "Month");

        private TimeFrame(int id, string name)
            : base(id, name)
        {
        }

        public long TimestampMillisecond => Id * 1000;
        public long TimestampSecond => Id;
        public long TimestampMinute => Id / 60;

        public long RoundingTimeStamp(long timeStamp)
        {
            var dt = ConvertDateTime.UnixTimeStampMillisecondsToDateTime(timeStamp);
            switch (Id)
            {
                case 1: // секунды
                    {
                        var sec = Id * (dt.Second / Id);
                        var dtNew = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, sec, 0, DateTimeKind.Utc);
                        return ConvertDateTime.DateToUnixTimeStampMilliseconds(dtNew);
                    }
                case 60: // минуты
                case 180:
                case 300:
                case 900:
                case 1800:
                    {
                        var id = Id / 60;
                        var minute = id * (dt.Minute / id);
                        var dtNew = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, minute, 0, DateTimeKind.Utc);
                        return ConvertDateTime.DateToUnixTimeStampMilliseconds(dtNew);
                    }
                case 3600: // часы
                case 7200:
                case 10800:
                case 14400:
                case 21600:
                case 43200:
                    {
                        var id = Id / 3600;
                        var hour = id * (dt.Hour / id);
                        var dtNew = new DateTime(dt.Year, dt.Month, dt.Day, hour, 0, 0, DateTimeKind.Utc);
                        return ConvertDateTime.DateToUnixTimeStampMilliseconds(dtNew);
                    }
                case 86400: // день
                    {
                        var dtNew = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Utc);
                        return ConvertDateTime.DateToUnixTimeStampMilliseconds(dtNew);
                    }
                case 604800: // неделя
                    {
                        var dayOfWeek = (int)dt.DayOfWeek;
                        if (dayOfWeek == 0) dayOfWeek = 7;
                        var week = dayOfWeek - 1;
                        dt = dt.AddDays(-week);
                        var dtNew = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Utc);
                        return ConvertDateTime.DateToUnixTimeStampMilliseconds(dtNew);
                    }
                case 2149200: // месяц
                    {
                        var dtNew = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                        return ConvertDateTime.DateToUnixTimeStampMilliseconds(dtNew);
                    }
            }

            throw new NotImplementedException($"Нет реализации округления TimeStamp для TimeFrame = {Name}");
        }

        public static IEnumerable<TimeFrame> List()
        {
            return new[]
            {
                Sec1,
                Min1,
                Min3,
                Min5,
                Min15,
                Min30,
                Hour1,
                Hour2,
                Hour3,
                Hour4,
                Hour6,
                Hour12,
                Day,
                Week,
                Month
            };
        }

        public static TimeFrame FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(TimeFrame).Name}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static TimeFrame From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException(
                    $"Возможные значения для {typeof(TimeFrame).Name}: {string.Join(",", List().Select(s => s.Id))}");
            }

            return state;
        }
    }
}
