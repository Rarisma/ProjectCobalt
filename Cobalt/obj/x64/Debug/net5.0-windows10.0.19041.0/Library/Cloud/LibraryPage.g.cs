﻿#pragma checksum "A:\Repositories\Cobalt\Cobalt\Library\Cloud\LibraryPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E426406E678DAC0E8895368A5C563E4E109D302F3CAAC30FF3DFE7DBE4E17BE4"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using WinRT;

namespace Cobalt.Library.Cloud
{
    partial class LibraryPage : 
        global::Microsoft.UI.Xaml.Controls.Page, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 0.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // Library\Cloud\LibraryPage.xaml line 19
                {
                    this.SideBar = target.As<Microsoft.UI.Xaml.Controls.ListBox>();
                    ((global::Microsoft.UI.Xaml.Controls.ListBox)this.SideBar).SelectionChanged += this.SelectedGameUpdate;
                }
                break;
            case 3: // Library\Cloud\LibraryPage.xaml line 22
                {
                    this.Name = target.As<Microsoft.UI.Xaml.Controls.TextBlock>();
                }
                break;
            case 4: // Library\Cloud\LibraryPage.xaml line 23
                {
                    this.Description = target.As<Microsoft.UI.Xaml.Controls.TextBlock>();
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 0.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

