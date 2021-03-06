﻿/*
 * Copyright (c) Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * ALSO SEE: https://github.com/SoftwarePotential/samples/wiki/License
 * 
 */
namespace Sp.Samples.Agent.WpfApplication.Activation
{
	using System;
	using System.ComponentModel;

	public class ActivationModel : IDataErrorInfo, INotifyPropertyChanged
	{
		string _activationKey;

		public string ActivationKey
		{
			get { return _activationKey; }
			set
			{
				_activationKey = value;
				OnPropertyChanged( "ActivationKey" );
			}
		}

		public void ActivateOnline()
		{
			SpAgent.ActivateLicense( ActivationKey );
		}

		static bool IsActivationKeyWellFormed( string activationKey )
		{
			return SpAgent.IsActivationKeyWellFormed( activationKey );
		}

		public static int ActivationKeyRequiredLength
		{
			get { return 29; }
		}

		static string ValidateActivationKey( string activationKey )
		{
			string result = null;
			if ( string.IsNullOrEmpty( activationKey ) )
				result = "Please enter an activation key";
			else if ( activationKey.Length != ActivationKeyRequiredLength )
				result = string.Format( "Activation key should be exactly {0} characters long", ActivationKeyRequiredLength );
			else if ( !IsActivationKeyWellFormed( activationKey ) )
				result = "Activation key is in incorrect format";
			return result;
		}

		#region IDataErrorInfo Members
		public string this[ string columnName ]
		{
			get
			{
				string result = null;
				if ( columnName == "ActivationKey" )
				{
					result = ValidateActivationKey( ActivationKey );
				}
				return result;
			}
		}

		public string Error
		{
			get { return null; }
		}
		#endregion

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged( String propertyName )
		{
			if ( PropertyChanged != null )
			{
				PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}
		#endregion
	}
}
