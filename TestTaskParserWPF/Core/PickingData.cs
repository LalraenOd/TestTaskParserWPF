using System;

namespace TestTaskParserWPF.Core
{
    internal class PickingData
    {
        public PickingData()
        {
        }

        public PickingData(string subGroupLink, string imageName)
        {
            SubGroupLink = subGroupLink;
            ImageName = imageName;
        }

        public PickingData(string treeCode, string tree, string number, int quantity, string dateRange, string info, string subGroupLink, string imageName)
        {
            TreeCode = treeCode ?? throw new ArgumentNullException(nameof(treeCode));
            Tree = tree ?? throw new ArgumentNullException(nameof(tree));
            Number = number ?? throw new ArgumentNullException(nameof(number));
            Quantity = quantity;
            DateRange = dateRange ?? throw new ArgumentNullException(nameof(dateRange));
            Info = info ?? throw new ArgumentNullException(nameof(info));
            SubGroupLink = subGroupLink ?? throw new ArgumentNullException(nameof(subGroupLink));
            ImageName = imageName ?? throw new ArgumentNullException(nameof(imageName));
        }

        internal string TreeCode { get; set; }
        internal string Tree { get; set; }
        internal string Number { get; set; }
        internal int Quantity { get; set; }
        internal string DateRange { get; set; }
        internal string Info { get; set; }
        internal string SubGroupLink { get; set; }
        internal string ImageName { get; set; }
    }
}