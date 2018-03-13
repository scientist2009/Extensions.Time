using System;

namespace Extensions.Time
{
    public static class TimeExtensions
    {
        private readonly static DateTime _unixTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970,1,1), TimeZoneInfo.Utc);
        private readonly static DateTimeOffset _unixTimeDto = new DateTimeOffset(1970, 1,1,0,0,0,0, TimeZoneInfo.Utc.BaseUtcOffset);

        public static DateTime AsLocalTime(this DateTime d)
        {
            switch(d.Kind)
            {
                case DateTimeKind.Local: return d;
                case DateTimeKind.Utc : return d.ToLocalTime();
                case DateTimeKind.Unspecified : return TimeZoneInfo.ConvertTimeToUtc(d, TimeZoneInfo.Local).ToLocalTime();
                default:
                    throw new NotSupportedException("不支持的d.Kind");
            }
        }

        public static DateTime AsUtcTime(this DateTime d)
        {
            switch(d.Kind)
            {
                case DateTimeKind.Utc : return d;
                case DateTimeKind.Local: return d.ToUniversalTime();
                case DateTimeKind.Unspecified : return TimeZoneInfo.ConvertTimeToUtc(d, TimeZoneInfo.Utc);
                default:
                    throw new NotSupportedException("不支持的d.Kind");
            }
        }

        public static long LocalTimeToUnixTimestamp(this DateTime localTime)
        {
            if (localTime.Kind != DateTimeKind.Local && localTime.Kind != DateTimeKind.Unspecified)
            {
                throw new ArgumentOutOfRangeException("localTime", "locatTime.Kind只能为Local或Unspecified");
            }
            DateTime dt = localTime;
            if (localTime.Kind == DateTimeKind.Unspecified)
            {
                dt = localTime.AsLocalTime();
            }
            return Convert.ToInt64((dt.ToUniversalTime() - _unixTime).TotalSeconds);
        }

        public static long UtcTimeToUnixTimestamp(this DateTime utcTime)
        {
            if (utcTime.Kind != DateTimeKind.Utc && utcTime.Kind!= DateTimeKind.Unspecified)
            {
                throw new ArgumentOutOfRangeException("utcTime", "utcTime.Kind只能为Utc或Unspecified");
            }
            DateTime dt = utcTime;
            if (utcTime.Kind == DateTimeKind.Unspecified)
            {
                dt = utcTime.AsUtcTime();
            }
            return Convert.ToInt64((dt - _unixTime).TotalSeconds);
        }

        public static DateTime UnixTimestampToUtcTime(this long unix)
        {
            return _unixTime.AddSeconds(unix);
        }

        public static DateTime UnixTimestampToLocalTime(this long unix)
        {
            return _unixTime.AddSeconds(unix).ToLocalTime();
        }

        private static long ToJsTimestamp(this DateTime d)
        {
            if (d.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentOutOfRangeException("d", "d.Kind必须为Utc");
            }
            return Convert.ToInt64((d - _unixTime).TotalMilliseconds);
        }

        public static long LocalTimeToJsTimestamp(this DateTime localTime)
        {
            if (localTime.Kind != DateTimeKind.Local && localTime.Kind != DateTimeKind.Unspecified)
            {
                throw new ArgumentOutOfRangeException("localTime", "localTime.Kind只能为Local或Unspecified");
            }
            return localTime.AsLocalTime().ToUniversalTime().ToJsTimestamp();
        }

        public static long UtcTimeToJsTimestamp(this DateTime utcTime)
        {
            if (utcTime.Kind != DateTimeKind.Utc && utcTime.Kind != DateTimeKind.Unspecified)
            {
                throw new ArgumentOutOfRangeException("utcTime", "utcTime.Kind只能为Utc或Unspecified");
            }
            return utcTime.AsUtcTime().ToJsTimestamp();
        }

        public static DateTime JsTimestampToUtcTime(this long js)
        {
            return _unixTime.AddMilliseconds(js);
        }

        public static DateTime JsTimestampToLocalTime(this long js)
        {
            return js.JsTimestampToUtcTime().ToLocalTime();
        }

        public static DateTimeOffset UtcTicksToUtcTimeOffset(this long utcTicks)
        {
            return new DateTimeOffset(utcTicks, TimeZoneInfo.Utc.BaseUtcOffset);
        }

        public static DateTimeOffset UtcTicksToLocalTimeOffset(this long utcTicks)
        {
            return utcTicks.UtcTicksToUtcTimeOffset().ToLocalTime();
        }

        public static DateTime UtcTicksToUtcTime(this long utcTicks)
        {
            return utcTicks.UtcTicksToUtcTimeOffset().UtcDateTime;
        }

        public static DateTime UtcTicksToLocalTime(this long utcTicks)
        {
            return utcTicks.UtcTicksToUtcTimeOffset().LocalDateTime;
        }

        public static long LocalTimeToUtcTicks(this DateTime localTime)
        {
            if (localTime.Kind != DateTimeKind.Local && localTime.Kind != DateTimeKind.Unspecified)
            {
                throw new ArgumentOutOfRangeException("localTime", "localTime.Kind必须为Local或Unspecified");
            }
            return localTime.AsLocalTime().ToUniversalTime().Ticks;
        }

        public static long UtcTimeToUtcTicks(this DateTime utcTime)
        {
            if (utcTime.Kind != DateTimeKind.Utc && utcTime.Kind != DateTimeKind.Unspecified)
            {
                throw new ArgumentOutOfRangeException("utcTime", "utcTime.Kind必须为Local或Unspecified");
            }
            return utcTime.AsUtcTime().Ticks;
        }

        public static long UnixTimestampToUtcTicks(this long unix)
        {
            return unix * TimeSpan.TicksPerSecond + _unixTime.Ticks;
        }

        public static long UtcTicksToUnixTimestamp(this long utcTicks)
        {
            return (utcTicks - _unixTime.Ticks)/TimeSpan.TicksPerSecond;
        }

        public static long JsTimestampToUtcTicks(this long js)
        {
            long ms = js % 1000;
            long s = js / 1000;
            return _unixTime.AddSeconds(s).AddMilliseconds(ms).UtcTimeToUtcTicks();
        }

        public static long UtcTicksToJsTimestamp(this long utcTicks)
        {
            return utcTicks.UtcTicksToUtcTime().ToJsTimestamp();
        }

        public static long JsTimestampToUnixTimestamp(this long js)
        {
            return js.JsTimestampToUtcTime().UtcTimeToUnixTimestamp();
        }

        public static long UnixTimestampToJsTimestamp(this long unix)
        {
            return unix.UnixTimestampToUtcTime().UtcTimeToJsTimestamp();
        }

        public static DateTimeOffset JsTimestampToLocalTimeOffset(this long js)
        {
            return js.JsTimestampToUtcTicks().UtcTicksToLocalTimeOffset();
        }

        public static DateTimeOffset JsTimestampToUtcTimeOffset(this long js)
        {
            return js.JsTimestampToUtcTicks().UtcTicksToUtcTimeOffset();
        }
    }
}
