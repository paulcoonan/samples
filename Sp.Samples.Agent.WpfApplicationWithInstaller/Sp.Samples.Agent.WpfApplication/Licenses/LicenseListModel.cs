﻿/*
 * Copyright (c) Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * ALSO SEE: https://github.com/SoftwarePotential/samples/wiki/License
 * 
 */
namespace Sp.Samples.Agent.WpfApplication.Licenses
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.Linq;
	using System.Windows.Data;
	using Sp.Agent;

	public class LicenseListModel
	{
		public string ProductName { get; private set; }
		public string ProductVersion { get; private set; }
		public ObservableCollection<LicenseItemModel> Licenses { get; set; }

		public LicenseListModel(IEnumerable<LicenseItemModel> licenses, string productName, string productVersion )
		{
			ProductName = productName;
			ProductVersion = productVersion;
			Licenses = new ObservableCollection<LicenseItemModel>(licenses);
		}

		public void RemoveLicenseItem( LicenseItemModel license )
		{
			// Remove license from license store
			SpAgent.RemoveLicense( license.ActivationKey );
			// Remove license from the model
			Licenses.Remove( license );
		}
	}

	public class LicenseItemModel
	{
		public string ActivationKey { get; set; }
		public DateTime ValidUntil { get; set; }
		public IEnumerable<string> Features { get; set; }
	}

	public class LicenseListModelFactory
	{
		public LicenseListModel CreateLicenseListModel()
		{
			IProductContext productContext = SpAgent.Product;

			// If there's no product context, then probably SpAgent hasn't been initialized 
			// (this can happen inside Visual Studio Designer)
			if(productContext == null)
				return new LicenseListModel( new LicenseItemModel[]{}, "Unknown Product", "Unknown Version" );

			IEnumerable<LicenseItemModel> licenseItems = RetrieveAllLicenses( productContext );
			LicenseListModel licenseListModel = new LicenseListModel( licenseItems, productContext.ProductName, productContext.ProductVersion );

			return licenseListModel;
		}

		IEnumerable<LicenseItemModel> RetrieveAllLicenses( IProductContext productContext )
		{
			return 
				productContext.Licenses.All()
				.Select( l => new LicenseItemModel()
				{
					ActivationKey = l.ActivationKey,
					ValidUntil = l.ValidUntil,
					Features = l.Advanced.AllFeatures().Select( f => f.Key )
				} );
		}
	}

	#region Converters
	[ValueConversion( typeof( IEnumerable<string> ), typeof( string ) )]
	public class FlatStringArrayConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string separator = (string) parameter ?? ",";
			return string.Join( separator, (IEnumerable<string>)value );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}

	public class MultiValueConverter : IMultiValueConverter
	{
		public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
		{
			return values.Clone();
		}

		public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
	#endregion
}