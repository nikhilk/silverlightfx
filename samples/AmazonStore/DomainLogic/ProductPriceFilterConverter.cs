// ProductPriceFilterConverter.cs
//

using System;
using System.Globalization;
using System.Windows.Data;

namespace Store {

    public sealed class ProductPriceFilterConverter : IValueConverter {

        #region IValueConverter Members
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            ProductPriceFilter filter = (ProductPriceFilter)value;
            if (filter == null) {
                return Int32.MaxValue;
            }

            return filter.PriceRange;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            int priceRange = Convert.ToInt32(value);
            if (priceRange == 0) {
                return null;
            }

            return new ProductPriceFilter(priceRange);
        }
        #endregion
    }
}
