﻿#pragma checksum "A:\Repositories\Cobalt\Cobalt\Scanner.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "CE98B6863ED25B8A37D3A3E2D7C46E1845D83848090BCBDA96CCDF908F05BDF6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using WinRT;

namespace Cobalt
{
    partial class Scanner : 
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
            case 2: // Scanner.xaml line 14
                {
                    this.Gamedirs = target.As<Microsoft.UI.Xaml.Controls.TextBlock>();
                }
                break;
            case 3: // Scanner.xaml line 15
                {
                    this.RomsList = target.As<Microsoft.UI.Xaml.Controls.TextBox>();
                }
                break;
            case 4: // Scanner.xaml line 16
                {
                    this.ContinueButton = target.As<Microsoft.UI.Xaml.Controls.Button>();
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.ContinueButton).Click += this.Continue;
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

