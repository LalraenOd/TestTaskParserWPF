using System;

namespace TestTaskParserWPF.Core
{
    /// <summary>
    /// Class for storing picking data information (5-th parser step)
    /// </summary>
    internal class PickingData
    {
        public PickingData()
        {
        }

        public PickingData(string subGropName, string subGroupLink, string imageName)
        {
            SubGropName = subGropName ?? throw new ArgumentNullException(nameof(subGropName));
            SubGroupLink = subGroupLink ?? throw new ArgumentNullException(nameof(subGroupLink));
            ImageName = imageName ?? throw new ArgumentNullException(nameof(imageName));
        }

        public PickingData(string treeCode, string tree, string number, int quantity, string dateRange, string info, string subGropName, string subGroupLink, string imageName) : this(treeCode, tree, number)
        {
            Quantity = quantity;
            DateRange = dateRange ?? throw new ArgumentNullException(nameof(dateRange));
            Info = info ?? throw new ArgumentNullException(nameof(info));
            SubGropName = subGropName ?? throw new ArgumentNullException(nameof(subGropName));
            SubGroupLink = subGroupLink ?? throw new ArgumentNullException(nameof(subGroupLink));
            ImageName = imageName ?? throw new ArgumentNullException(nameof(imageName));
        }

        internal string TreeCode { get; set; }
        internal string Tree { get; set; }
        internal string Number { get; set; }
        internal int Quantity { get; set; }
        internal string DateRange { get; set; }
        internal string Info { get; set; }
        internal string SubGropName { get; set; }
        internal string SubGroupLink { get; set; }
        internal string ImageName { get; set; }
    }
}