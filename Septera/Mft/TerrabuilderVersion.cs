using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Septera
{
    public sealed class TerrabuilderVersion
    {
        public Decimal Engine { get; }
        public Int32 LV { get; }
        public Int32 AM { get; }
        public Int32 GV { get; }
        public Int32 TX { get; }
        public Int32 CH { get; }
        public Int32 IL { get; }

        public TerrabuilderVersion(String line)
        {
            Match result = ParsingExpression.Match(line);
            if (!result.Success)
                throw new NotSupportedException($"Cannot parse Terrabuilder version from [{line}]");

            Engine = result.ToDecimal(1);
            LV = result.ToInt32(2);
            AM = result.ToInt32(3);
            GV = result.ToInt32(4);
            TX = result.ToInt32(5);
            CH = result.ToInt32(6);
            IL = result.ToInt32(7);

            Validate();
        }

        private void Validate()
        {
            if (SupportedEngineVersions.All(v => v != Engine))
                throw new NotSupportedException($"Not supported engine version: {Engine}. Supported: {String.Join(", ", SupportedEngineVersions)}");

            //Assert(mft.ReadLine(), "Terrabuilder version 0.9802 LV25 AM04 GV00 TX00 CH14 IL00 ");

            CheckSupported(nameof(LV), LV, supported: 25);
            CheckSupported(nameof(AM), AM, supported: 04);
            CheckSupported(nameof(GV), GV, supported: 00);
            CheckSupported(nameof(TX), TX, supported: 00);
            CheckSupported(nameof(CH), CH, supported: 14);
            CheckSupported(nameof(IL), IL, supported: 00);

            void CheckSupported(String name, Int32 value, Int32 supported)
            {
                if (value != supported)
                    throw new NotSupportedException($"Not supported {name} version: {value}. Supported: {supported}");
            }
        }

        private static readonly Decimal[] SupportedEngineVersions = {0.9802m, 1.0000m};
        private static readonly Regex ParsingExpression = new Regex("Terrabuilder version ([.0-9]+) LV([0-9]{2}) AM([0-9]{2}) GV([0-9]{2}) TX([0-9]{2}) CH([0-9]{2}) IL([0-9]{2})", RegexOptions.Compiled);
    }
}