
using System.Collections.Generic;
using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using INTV.Shared.View;

namespace INTV.JzIntvUI.View
{
    public partial class JzIntvSettingsPage : NSView, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public JzIntvSettingsPage(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public JzIntvSettingsPage(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }
        
        /// <summary>Shared initialization code.</summary>
        void Initialize()
        {
        }

        #endregion // Constructors

        /// <summary>
        /// Controller for the view.
        /// </summary>
        internal JzIntvSettingsPageController Controller { get; set; }

        #region IFakeDependencyObject

        /// <inheritdoc />
        public object DataContext
        {
            get { return Controller.DataContext; }
            set { Controller.DataContext = value; }
        }

        /// <inheritdoc />
        public object GetValue(string propertyName)
        {
            return Controller.GetValue(propertyName);
        }

        /// <inheritdoc />
        public void SetValue(string propertyName, object value)
        {
            Controller.SetValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject
    }
}
