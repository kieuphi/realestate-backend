using System;
using System.Collections.Generic;
using System.Drawing;

namespace General.Domain.Common.Excel
{
    public class ExcelReportFormatting
    {
        public ExcelFormat? Format { get; protected set; }

        public Color? TextColor { get; set; }

        public Color? BackgroundColor { get; set; }

        public float? FontSize { get; set; }

        public bool IsBold { get; set; }

        public bool IsItalics { get; set; }

        public bool IsUnderlined { get; set; }
    }

    public class ExcelReportHeader : ExcelReportFormatting
    {
        public string FieldName { get; }

        public string DisplayName { get; }

        public string DefaultValue { get; }

        public Type Type { get; }

        public List<string> ChildListValues { get; }

        public string ChildListOperator { get; }

        public string Formula { get; }

        /// <summary>
        ///     Only specify the field name
        /// </summary>
        /// <param name="field"></param>
        public ExcelReportHeader(string field)
        {
            FieldName = field;
        }

        /// <summary>
        ///     Specify the field name and the display name
        /// </summary>
        /// <param name="field"></param>
        /// <param name="display"></param>
        public ExcelReportHeader(string field, string display)
        {
            FieldName = field;
            DisplayName = display;
        }

        /// <summary>
        ///     Specify the field name and the ExcelFormat
        /// </summary>
        /// <param name="field"></param>
        /// <param name="format"></param>
        public ExcelReportHeader(string field, ExcelFormat format)
        {
            FieldName = field;
            Format = format;
        }

        /// <summary>
        ///     Specify the field name, the display name, and the ExcelFormat
        /// </summary>
        /// <param name="field"></param>
        /// <param name="display"></param>
        /// <param name="format"></param>
        public ExcelReportHeader(string field, string display, ExcelFormat format)
        {
            FieldName = field;
            DisplayName = display;
            Format = format;
        }

        /// <summary>
        ///     Used only for constant fields that are not part of the DTO. E.g. "EffectiveDate = Today"
        ///     Specify the display name, the ExcelFormat (nullable), and the value of the field
        /// </summary>
        /// <param name="display"></param>
        /// <param name="format"></param>
        /// <param name="defaultValue"></param>
        public ExcelReportHeader(string display, ExcelFormat? format, string defaultValue)
        {
            DisplayName = display;
            Format = format;
            DefaultValue = defaultValue;
        }

        /// <summary>
        ///     Used only for Enums that have an EnumMember Attribute
        ///     Specify the field name, the ExcelFormat, and the Enum
        /// </summary>
        /// <param name="field"></param>
        /// <param name="format"></param>
        /// <param name="type"></param>
        public ExcelReportHeader(string field, ExcelFormat format, Type type)
        {
            FieldName = field;
            Format = format;
            Type = type;
        }
    }

    public class ExcelTextAlign
    {
        public bool AlignCenter { get; set; }

        public bool AlignLeft { get; set; }

        public bool AlignRight { get; set; }
    }
}
