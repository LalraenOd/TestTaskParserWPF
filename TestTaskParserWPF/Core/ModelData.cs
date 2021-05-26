using System;

namespace TestTaskParserWPF.Core
{
    internal class ModelData
    {
        internal ModelData(string modelCode, string modelName, string modelDateRange, string modelPickingCode)
        {
            ModelCode = modelCode ?? throw new ArgumentNullException(nameof(modelCode));
            ModelName = modelName ?? throw new ArgumentNullException(nameof(modelName));
            ModelDateRange = modelDateRange ?? throw new ArgumentNullException(nameof(modelDateRange));
            ModelPickingCode = modelPickingCode ?? throw new ArgumentNullException(nameof(modelPickingCode));
        }

        internal string ModelCode { get; set; }

        internal string ModelName { get; set; }

        internal string ModelDateRange { get; set; }

        internal string ModelPickingCode { get; set; }
    }
}