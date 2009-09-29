﻿//-----------------------------------------------------------------------
// <copyright file="OpenIdAjaxTextBox.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: System.Web.UI.WebResource(DotNetOpenAuth.OpenId.RelyingParty.OpenIdAjaxTextBox.EmbeddedScriptResourceName, "text/javascript")]
[assembly: System.Web.UI.WebResource(DotNetOpenAuth.OpenId.RelyingParty.OpenIdAjaxTextBox.EmbeddedDotNetOpenIdLogoResourceName, "image/gif")]
[assembly: System.Web.UI.WebResource(DotNetOpenAuth.OpenId.RelyingParty.OpenIdAjaxTextBox.EmbeddedSpinnerResourceName, "image/gif")]
[assembly: System.Web.UI.WebResource(DotNetOpenAuth.OpenId.RelyingParty.OpenIdAjaxTextBox.EmbeddedLoginSuccessResourceName, "image/png")]
[assembly: System.Web.UI.WebResource(DotNetOpenAuth.OpenId.RelyingParty.OpenIdAjaxTextBox.EmbeddedLoginFailureResourceName, "image/png")]

#pragma warning disable 0809 // marking inherited, unsupported properties as obsolete to discourage their use

namespace DotNetOpenAuth.OpenId.RelyingParty {
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.Text;
	using System.Web.UI;
	using DotNetOpenAuth.Messaging;

	/// <summary>
	/// An ASP.NET control that provides a minimal text box that is OpenID-aware and uses AJAX for
	/// a premium login experience.
	/// </summary>
	[DefaultProperty("Text"), ValidationProperty("Text")]
	[ToolboxData("<{0}:OpenIdAjaxTextBox runat=\"server\" />")]
	public class OpenIdAjaxTextBox : OpenIdRelyingPartyAjaxControlBase, ICallbackEventHandler, IEditableTextControl, ITextControl, IPostBackDataHandler {
		/// <summary>
		/// The name of the manifest stream containing the OpenIdAjaxTextBox.js file.
		/// </summary>
		internal const string EmbeddedScriptResourceName = Util.DefaultNamespace + ".OpenId.RelyingParty.OpenIdAjaxTextBox.js";

		/// <summary>
		/// The name of the manifest stream containing the dotnetopenid_16x16.gif file.
		/// </summary>
		internal const string EmbeddedDotNetOpenIdLogoResourceName = Util.DefaultNamespace + ".OpenId.RelyingParty.dotnetopenid_16x16.gif";

		/// <summary>
		/// The name of the manifest stream containing the spinner.gif file.
		/// </summary>
		internal const string EmbeddedSpinnerResourceName = Util.DefaultNamespace + ".OpenId.RelyingParty.spinner.gif";

		/// <summary>
		/// The name of the manifest stream containing the login_success.png file.
		/// </summary>
		internal const string EmbeddedLoginSuccessResourceName = Util.DefaultNamespace + ".OpenId.RelyingParty.login_success.png";

		/// <summary>
		/// The name of the manifest stream containing the login_failure.png file.
		/// </summary>
		internal const string EmbeddedLoginFailureResourceName = Util.DefaultNamespace + ".OpenId.RelyingParty.login_failure.png";

		#region Property viewstate keys

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="Columns"/> property.
		/// </summary>
		private const string ColumnsViewStateKey = "Columns";

		/// <summary>
		/// The viewstate key to use for the <see cref="CssClass"/> property.
		/// </summary>
		private const string CssClassViewStateKey = "CssClass";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="OnClientAssertionReceived"/> property.
		/// </summary>
		private const string OnClientAssertionReceivedViewStateKey = "OnClientAssertionReceived";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="AuthenticatedAsToolTip"/> property.
		/// </summary>
		private const string AuthenticatedAsToolTipViewStateKey = "AuthenticatedAsToolTip";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="AuthenticationSucceededToolTip"/> property.
		/// </summary>
		private const string AuthenticationSucceededToolTipViewStateKey = "AuthenticationSucceededToolTip";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="LogOnInProgressMessage"/> property.
		/// </summary>
		private const string LogOnInProgressMessageViewStateKey = "BusyToolTip";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="AuthenticationFailedToolTip"/> property.
		/// </summary>
		private const string AuthenticationFailedToolTipViewStateKey = "AuthenticationFailedToolTip";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="IdentifierRequiredMessage"/> property.
		/// </summary>
		private const string IdentifierRequiredMessageViewStateKey = "BusyToolTip";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="BusyToolTip"/> property.
		/// </summary>
		private const string BusyToolTipViewStateKey = "BusyToolTip";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="LogOnText"/> property.
		/// </summary>
		private const string LogOnTextViewStateKey = "LoginText";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="Throttle"/> property.
		/// </summary>
		private const string ThrottleViewStateKey = "Throttle";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="LogOnToolTip"/> property.
		/// </summary>
		private const string LogOnToolTipViewStateKey = "LoginToolTip";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="Name"/> property.
		/// </summary>
		private const string NameViewStateKey = "Name";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="Timeout"/> property.
		/// </summary>
		private const string TimeoutViewStateKey = "Timeout";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="TabIndex"/> property.
		/// </summary>
		private const string TabIndexViewStateKey = "TabIndex";

		/// <summary>
		/// The viewstate key to use for the <see cref="Enabled"/> property.
		/// </summary>
		private const string EnabledViewStateKey = "Enabled";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="RetryToolTip"/> property.
		/// </summary>
		private const string RetryToolTipViewStateKey = "RetryToolTip";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="RetryText"/> property.
		/// </summary>
		private const string RetryTextViewStateKey = "RetryText";

		/// <summary>
		/// The viewstate key to use for storing the value of the <see cref="EnableSplitButtonLogin"/> property.
		/// </summary>
		private const string EnableSplitButtonLoginViewStateKey = "EnableSplitButtonLogin";

		#endregion

		#region Property defaults

		/// <summary>
		/// The default value for the <see cref="Columns"/> property.
		/// </summary>
		private const int ColumnsDefault = 40;

		/// <summary>
		/// The default value for the <see cref="CssClass"/> property.
		/// </summary>
		private const string CssClassDefault = "openid";

		/// <summary>
		/// The default value for the <see cref="LogOnInProgressMessage"/> property.
		/// </summary>
		private const string LogOnInProgressMessageDefault = "Please wait for login to complete.";

		/// <summary>
		/// The default value for the <see cref="AuthenticationSucceededToolTip"/> property.
		/// </summary>
		private const string AuthenticationSucceededToolTipDefault = "Authenticated by {0}.";

		/// <summary>
		/// The default value for the <see cref="AuthenticatedAsToolTip"/> property.
		/// </summary>
		private const string AuthenticatedAsToolTipDefault = "Authenticated as {0}.";

		/// <summary>
		/// The default value for the <see cref="AuthenticationFailedToolTip"/> property.
		/// </summary>
		private const string AuthenticationFailedToolTipDefault = "Authentication failed.";

		/// <summary>
		/// The default value for the <see cref="Throttle"/> property.
		/// </summary>
		private const int ThrottleDefault = 3;

		/// <summary>
		/// The default value for the <see cref="LogOnText"/> property.
		/// </summary>
		private const string LogOnTextDefault = "LOG IN";

		/// <summary>
		/// The default value for the <see cref="BusyToolTip"/> property.
		/// </summary>
		private const string BusyToolTipDefault = "Discovering/authenticating";

		/// <summary>
		/// The default value for the <see cref="IdentifierRequiredMessage"/> property.
		/// </summary>
		private const string IdentifierRequiredMessageDefault = "Please correct errors in OpenID identifier and allow login to complete before submitting.";

		/// <summary>
		/// The default value for the <see cref="Name"/> property.
		/// </summary>
		private const string NameDefault = "openid_identifier";

		/// <summary>
		/// Default value for <see cref="TabIndex"/> property.
		/// </summary>
		private const short TabIndexDefault = 0;

		/// <summary>
		/// The default value for the <see cref="RetryToolTip"/> property.
		/// </summary>
		private const string RetryToolTipDefault = "Retry a failed identifier discovery.";

		/// <summary>
		/// The default value for the <see cref="LogOnToolTip"/> property.
		/// </summary>
		private const string LogOnToolTipDefault = "Click here to log in using a pop-up window.";

		/// <summary>
		/// The default value for the <see cref="RetryText"/> property.
		/// </summary>
		private const string RetryTextDefault = "RETRY";

		/// <summary>
		/// The default vlaue for the <see cref="EnableSplitButtonLogin"/> property.
		/// </summary>
		private const bool EnableSplitButtonLoginDefault = true;

		#endregion

		/// <summary>
		/// The path where the YUI control library should be downloaded from for HTTP pages.
		/// </summary>
		private const string YuiLoaderHttp = "http://ajax.googleapis.com/ajax/libs/yui/2.8.0r4/build/yuiloader/yuiloader-min.js";

		/// <summary>
		/// The path where the YUI control library should be downloaded from for HTTPS pages.
		/// </summary>
		private const string YuiLoaderHttps = "https://ajax.googleapis.com/ajax/libs/yui/2.8.0r4/build/yuiloader/yuiloader-min.js";

		#region Events

		/// <summary>
		/// Fired when the content of the text changes between posts to the server.
		/// </summary>
		[Description("Occurs when the content of the text changes between posts to the server."), Category(BehaviorCategory)]
		public event EventHandler TextChanged;

		/// <summary>
		/// Gets or sets the client-side script that executes when an authentication
		/// assertion is received (but before it is verified).
		/// </summary>
		/// <remarks>
		/// <para>In the context of the executing javascript set in this property, the 
		/// local variable <i>sender</i> is set to the openid_identifier input box
		/// that is executing this code.  
		/// This variable has a getClaimedIdentifier() method that may be used to
		/// identify the user who is being authenticated.</para>
		/// <para>It is <b>very</b> important to note that when this code executes,
		/// the authentication has not been verified and may have been spoofed.
		/// No security-sensitive operations should take place in this javascript code.
		/// The authentication is verified on the server by the time the 
		/// <see cref="OpenIdRelyingPartyControlBase.LoggedIn"/> server-side event fires.</para>
		/// </remarks>
		[Description("Gets or sets the client-side script that executes when an authentication assertion is received (but before it is verified).")]
		[Bindable(true), DefaultValue(""), Category(BehaviorCategory)]
		public string OnClientAssertionReceived {
			get { return this.ViewState[OnClientAssertionReceivedViewStateKey] as string; }
			set { this.ViewState[OnClientAssertionReceivedViewStateKey] = value; }
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the value in the text field, completely unprocessed or normalized.
		/// </summary>
		[Bindable(true), DefaultValue(""), Category(AppearanceCategory)]
		[Description("The content of the text box.")]
		public string Text {
			get { return this.Identifier != null ? this.Identifier.OriginalString : string.Empty; }
			set { this.Identifier = value; }
		}

		/// <summary>
		/// Gets or sets the width of the text box in characters.
		/// </summary>
		[Bindable(true), Category(AppearanceCategory), DefaultValue(ColumnsDefault)]
		[Description("The width of the text box in characters.")]
		public int Columns {
			get {
				return (int)(this.ViewState[ColumnsViewStateKey] ?? ColumnsDefault);
			}

			set {
				Contract.Requires(value >= 0);
				ErrorUtilities.VerifyArgumentInRange(value >= 0, "value");
				this.ViewState[ColumnsViewStateKey] = value;
			}
		}

		/// <summary>
		/// Gets or sets the CSS class assigned to the text box.
		/// </summary>
		[Bindable(true), DefaultValue(CssClassDefault), Category(AppearanceCategory)]
		[Description("The CSS class assigned to the text box.")]
		public string CssClass {
			get { return (string)this.ViewState[CssClassViewStateKey]; }
			set { this.ViewState[CssClassViewStateKey] = value; }
		}

		/// <summary>
		/// Gets or sets the tab index of the text box control.  Use 0 to omit an explicit tabindex.
		/// </summary>
		[Bindable(true), Category(BehaviorCategory), DefaultValue(TabIndexDefault)]
		[Description("The tab index of the text box control.  Use 0 to omit an explicit tabindex.")]
		public virtual short TabIndex {
			get { return (short)(this.ViewState[TabIndexViewStateKey] ?? TabIndexDefault); }
			set { this.ViewState[TabIndexViewStateKey] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="OpenIdTextBox"/> is enabled
		/// in the browser for editing and will respond to incoming OpenID messages.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		[Bindable(true), DefaultValue(true), Category(BehaviorCategory)]
		[Description("Whether the control is editable in the browser and will respond to OpenID messages.")]
		public bool Enabled {
			get { return (bool)(this.ViewState[EnabledViewStateKey] ?? true); }
			set { this.ViewState[EnabledViewStateKey] = value; }
		}

		/// <summary>
		/// Gets or sets the HTML name to assign to the text field.
		/// </summary>
		[Bindable(true), DefaultValue(NameDefault), Category("Misc")]
		[Description("The HTML name to assign to the text field.")]
		public string Name {
			get {
				return (string)(this.ViewState[NameViewStateKey] ?? NameDefault);
			}

			set {
				ErrorUtilities.VerifyNonZeroLength(value, "value");
				this.ViewState[NameViewStateKey] = value ?? string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the time duration for the AJAX control to wait for an OP to respond before reporting failure to the user.
		/// </summary>
		[Browsable(true), DefaultValue(typeof(TimeSpan), "00:00:08"), Category(BehaviorCategory)]
		[Description("The time duration for the AJAX control to wait for an OP to respond before reporting failure to the user.")]
		public TimeSpan Timeout {
			get {
				return (TimeSpan)(this.ViewState[TimeoutViewStateKey] ?? TimeoutDefault);
			}

			set {
				ErrorUtilities.VerifyArgumentInRange(value.TotalMilliseconds > 0, "value");
				this.ViewState[TimeoutViewStateKey] = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum number of OpenID Providers to simultaneously try to authenticate with.
		/// </summary>
		[Browsable(true), DefaultValue(ThrottleDefault), Category(BehaviorCategory)]
		[Description("The maximum number of OpenID Providers to simultaneously try to authenticate with.")]
		public int Throttle {
			get {
				return (int)(this.ViewState[ThrottleViewStateKey] ?? ThrottleDefault);
			}

			set {
				ErrorUtilities.VerifyArgumentInRange(value > 0, "value");
				this.ViewState[ThrottleViewStateKey] = value;
			}
		}

		/// <summary>
		/// Gets or sets the text that appears on the LOG IN button in cases where immediate (invisible) authentication fails.
		/// </summary>
		[Bindable(true), DefaultValue(LogOnTextDefault), Localizable(true), Category(AppearanceCategory)]
		[Description("The text that appears on the LOG IN button in cases where immediate (invisible) authentication fails.")]
		public string LogOnText {
			get {
				return (string)(this.ViewState[LogOnTextViewStateKey] ?? LogOnTextDefault);
			}

			set {
				ErrorUtilities.VerifyNonZeroLength(value, "value");
				this.ViewState[LogOnTextViewStateKey] = value ?? string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the rool tip text that appears on the LOG IN button in cases where immediate (invisible) authentication fails.
		/// </summary>
		[Bindable(true), DefaultValue(LogOnToolTipDefault), Localizable(true), Category(AppearanceCategory)]
		[Description("The tool tip text that appears on the LOG IN button in cases where immediate (invisible) authentication fails.")]
		public string LogOnToolTip {
			get { return (string)(this.ViewState[LogOnToolTipViewStateKey] ?? LogOnToolTipDefault); }
			set { this.ViewState[LogOnToolTipViewStateKey] = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets the text that appears on the RETRY button in cases where authentication times out.
		/// </summary>
		[Bindable(true), DefaultValue(RetryTextDefault), Localizable(true), Category(AppearanceCategory)]
		[Description("The text that appears on the RETRY button in cases where authentication times out.")]
		public string RetryText {
			get {
				return (string)(this.ViewState[RetryTextViewStateKey] ?? RetryTextDefault);
			}

			set {
				ErrorUtilities.VerifyNonZeroLength(value, "value");
				this.ViewState[RetryTextViewStateKey] = value ?? string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the tool tip text that appears on the RETRY button in cases where authentication times out.
		/// </summary>
		[Bindable(true), DefaultValue(RetryToolTipDefault), Localizable(true), Category(AppearanceCategory)]
		[Description("The tool tip text that appears on the RETRY button in cases where authentication times out.")]
		public string RetryToolTip {
			get { return (string)(this.ViewState[RetryToolTipViewStateKey] ?? RetryToolTipDefault); }
			set { this.ViewState[RetryToolTipViewStateKey] = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets the tool tip text that appears when authentication succeeds.
		/// </summary>
		[Bindable(true), DefaultValue(AuthenticationSucceededToolTipDefault), Localizable(true), Category(AppearanceCategory)]
		[Description("The tool tip text that appears when authentication succeeds.")]
		public string AuthenticationSucceededToolTip {
			get { return (string)(this.ViewState[AuthenticationSucceededToolTipViewStateKey] ?? AuthenticationSucceededToolTipDefault); }
			set { this.ViewState[AuthenticationSucceededToolTipViewStateKey] = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets the tool tip text that appears on the green checkmark when authentication succeeds.
		/// </summary>
		[Bindable(true), DefaultValue(AuthenticatedAsToolTipDefault), Localizable(true), Category(AppearanceCategory)]
		[Description("The tool tip text that appears on the green checkmark when authentication succeeds.")]
		public string AuthenticatedAsToolTip {
			get { return (string)(this.ViewState[AuthenticatedAsToolTipViewStateKey] ?? AuthenticatedAsToolTipDefault); }
			set { this.ViewState[AuthenticatedAsToolTipViewStateKey] = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets the tool tip text that appears when authentication fails.
		/// </summary>
		[Bindable(true), DefaultValue(AuthenticationFailedToolTipDefault), Localizable(true), Category(AppearanceCategory)]
		[Description("The tool tip text that appears when authentication fails.")]
		public string AuthenticationFailedToolTip {
			get { return (string)(this.ViewState[AuthenticationFailedToolTipViewStateKey] ?? AuthenticationFailedToolTipDefault); }
			set { this.ViewState[AuthenticationFailedToolTipViewStateKey] = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets the tool tip text that appears over the text box when it is discovering and authenticating.
		/// </summary>
		[Bindable(true), DefaultValue(BusyToolTipDefault), Localizable(true), Category(AppearanceCategory)]
		[Description("The tool tip text that appears over the text box when it is discovering and authenticating.")]
		public string BusyToolTip {
			get { return (string)(this.ViewState[BusyToolTipViewStateKey] ?? BusyToolTipDefault); }
			set { this.ViewState[BusyToolTipViewStateKey] = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets the message that is displayed if a postback is about to occur before the identifier has been supplied.
		/// </summary>
		[Bindable(true), DefaultValue(IdentifierRequiredMessageDefault), Localizable(true), Category(AppearanceCategory)]
		[Description("The message that is displayed if a postback is about to occur before the identifier has been supplied.")]
		public string IdentifierRequiredMessage {
			get { return (string)(this.ViewState[IdentifierRequiredMessageViewStateKey] ?? IdentifierRequiredMessageDefault); }
			set { this.ViewState[IdentifierRequiredMessageViewStateKey] = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets the message that is displayed if a postback is attempted while login is in process.
		/// </summary>
		[Bindable(true), DefaultValue(LogOnInProgressMessageDefault), Localizable(true), Category(AppearanceCategory)]
		[Description("The message that is displayed if a postback is attempted while login is in process.")]
		public string LogOnInProgressMessage {
			get { return (string)(this.ViewState[LogOnInProgressMessageViewStateKey] ?? LogOnInProgressMessageDefault); }
			set { this.ViewState[LogOnInProgressMessageViewStateKey] = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether a split button will be used for the "log in"
		/// when the user provides an identifier that delegates to more than one Provider.
		/// </summary>
		/// <value>
		/// 	<c>true</c> to use a split button; otherwise, <c>false</c> to use a standard HTML button.
		/// </value>
		/// <remarks>
		/// The split button brings in about 180KB of javascript dependencies.
		/// </remarks>
		[Bindable(true), DefaultValue(EnableSplitButtonLoginDefault), Category(BehaviorCategory)]
		[Description("Whether a split button will be used for the \"log in\" when the user provides an identifier that delegates to more than one Provider.")]
		public bool EnableSplitButtonLogin {
			get { return (bool)(this.ViewState[EnableSplitButtonLoginViewStateKey] ?? EnableSplitButtonLoginDefault); }
			set { this.ViewState[EnableSplitButtonLoginViewStateKey] = value; }
		}

		#endregion

		/// <summary>
		/// Gets the name of the open id auth data form key.
		/// </summary>
		/// <value>
		/// A concatenation of <see cref="Name"/> and <c>"_openidAuthData"</c>.
		/// </value>
		protected override string OpenIdAuthDataFormKey {
			get { return this.Name + "_openidAuthData"; }
		}

		/// <summary>
		/// Gets the default value for the <see cref="Timeout"/> property.
		/// </summary>
		/// <value>8 seconds; or eternity if the debugger is attached.</value>
		private static TimeSpan TimeoutDefault {
			get {
				if (Debugger.IsAttached) {
					Logger.OpenId.Warn("Debugger is attached.  Inflating default OpenIdAjaxTextbox.Timeout value to infinity.");
					return TimeSpan.MaxValue;
				} else {
					return TimeSpan.FromSeconds(8);
				}
			}
		}

		#region IPostBackDataHandler Members

		/// <summary>
		/// When implemented by a class, processes postback data for an ASP.NET server control.
		/// </summary>
		/// <param name="postDataKey">The key identifier for the control.</param>
		/// <param name="postCollection">The collection of all incoming name values.</param>
		/// <returns>
		/// true if the server control's state changes as a result of the postback; otherwise, false.
		/// </returns>
		bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) {
			return this.LoadPostData(postDataKey, postCollection);
		}

		/// <summary>
		/// When implemented by a class, signals the server control to notify the ASP.NET application that the state of the control has changed.
		/// </summary>
		void IPostBackDataHandler.RaisePostDataChangedEvent() {
			this.RaisePostDataChangedEvent();
		}

		#endregion

		/// <summary>
		/// Raises the <see cref="E:Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);

			this.Page.RegisterRequiresPostBack(this);
		}

		/// <summary>
		/// Prepares to render the control.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);

			if (this.EnableSplitButtonLogin) {
				string yuiLoadScript = @"var loader = new YAHOO.util.YUILoader({
	require: ['button', 'menu'],
	loadOptional: false,
	combine: true,
	onSuccess: function() { }
});

loader.insert();";
				this.Page.ClientScript.RegisterClientScriptInclude("yuiloader", this.Page.Request.Url.IsTransportSecure() ? YuiLoaderHttps : YuiLoaderHttp);
				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "requiredYuiComponents", yuiLoadScript, true);
			}

			this.PrepareClientJavascript();
		}

		/// <summary>
		/// Renders the control.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
		protected override void Render(HtmlTextWriter writer) {
			base.Render(writer);

			// We surround the textbox with a span so that the .js file can inject a
			// login button within the text box with easy placement.
			if (!string.IsNullOrEmpty(this.CssClass)) {
				writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);
			}

			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline-block");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
			writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "16px");
			writer.RenderBeginTag(HtmlTextWriterTag.Span);

			writer.AddAttribute(HtmlTextWriterAttribute.Name, this.Name);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			writer.AddAttribute(HtmlTextWriterAttribute.Size, this.Columns.ToString(CultureInfo.InvariantCulture));
			if (!string.IsNullOrEmpty(this.Text)) {
				writer.AddAttribute(HtmlTextWriterAttribute.Value, this.Text, true);
			}

			if (this.TabIndex > 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, this.TabIndex.ToString(CultureInfo.InvariantCulture));
			}
			if (!this.Enabled) {
				writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "true");
			}
			if (!string.IsNullOrEmpty(this.CssClass)) {
				writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);
			}
			writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "18px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "solid");
			writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "1px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "lightgray");
			writer.RenderBeginTag(HtmlTextWriterTag.Input);
			writer.RenderEndTag(); // </input>
			writer.RenderEndTag(); // </span>
		}

		/// <summary>
		/// When implemented by a class, processes postback data for an ASP.NET server control.
		/// </summary>
		/// <param name="postDataKey">The key identifier for the control.</param>
		/// <param name="postCollection">The collection of all incoming name values.</param>
		/// <returns>
		/// true if the server control's state changes as a result of the postback; otherwise, false.
		/// </returns>
		protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection) {
			// If the control was temporarily hidden, it won't be in the Form data,
			// and we'll just implicitly keep the last Text setting.
			if (postCollection[this.Name] != null) {
				Identifier identifier = postCollection[this.Name].Length == 0 ? null : postCollection[this.Name];
				if (identifier != this.Identifier) {
					this.Identifier = identifier;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// When implemented by a class, signals the server control to notify the ASP.NET application that the state of the control has changed.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Preserve signature of interface we're implementing.")]
		protected virtual void RaisePostDataChangedEvent() {
			this.OnTextChanged();
		}

		/// <summary>
		/// Called on a postback when the Text property has changed.
		/// </summary>
		protected virtual void OnTextChanged() {
			EventHandler textChanged = this.TextChanged;
			if (textChanged != null) {
				textChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Assembles the javascript to send to the client and registers it with ASP.NET for transmission.
		/// </summary>
		private void PrepareClientJavascript() {
			string identifierParameterName = "identifier";
			string discoveryCallbackResultParameterName = "resultFunction";
			string discoveryErrorCallbackParameterName = "errorCallback";
			string discoveryCallback = Page.ClientScript.GetCallbackEventReference(
				this,
				identifierParameterName,
				discoveryCallbackResultParameterName,
				identifierParameterName,
				discoveryErrorCallbackParameterName,
				true);

			// Import the .js file where most of the code is.
			this.Page.ClientScript.RegisterClientScriptResource(typeof(OpenIdAjaxTextBox), EmbeddedScriptResourceName);

			// Call into the .js file with initialization information.
			StringBuilder startupScript = new StringBuilder();
			startupScript.AppendLine("<script language='javascript'>");
			startupScript.AppendFormat("var box = document.getElementsByName('{0}')[0];{1}", this.Name, Environment.NewLine);
			startupScript.AppendFormat(
				CultureInfo.InvariantCulture,
				"initAjaxOpenId(box, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, function({18}, {19}, {20}) {{{21}}});{22}",
				MessagingUtilities.GetSafeJavascriptValue(this.Page.ClientScript.GetWebResourceUrl(this.GetType(), OpenIdTextBox.EmbeddedLogoResourceName)),
				MessagingUtilities.GetSafeJavascriptValue(this.Page.ClientScript.GetWebResourceUrl(this.GetType(), EmbeddedDotNetOpenIdLogoResourceName)),
				MessagingUtilities.GetSafeJavascriptValue(this.Page.ClientScript.GetWebResourceUrl(this.GetType(), EmbeddedSpinnerResourceName)),
				MessagingUtilities.GetSafeJavascriptValue(this.Page.ClientScript.GetWebResourceUrl(this.GetType(), EmbeddedLoginSuccessResourceName)),
				MessagingUtilities.GetSafeJavascriptValue(this.Page.ClientScript.GetWebResourceUrl(this.GetType(), EmbeddedLoginFailureResourceName)),
				this.Throttle,
				this.Timeout.TotalMilliseconds,
				string.IsNullOrEmpty(this.OnClientAssertionReceived) ? "null" : "'" + this.OnClientAssertionReceived.Replace(@"\", @"\\").Replace("'", @"\'") + "'",
				MessagingUtilities.GetSafeJavascriptValue(this.LogOnText),
				MessagingUtilities.GetSafeJavascriptValue(this.LogOnToolTip),
				MessagingUtilities.GetSafeJavascriptValue(this.RetryText),
				MessagingUtilities.GetSafeJavascriptValue(this.RetryToolTip),
				MessagingUtilities.GetSafeJavascriptValue(this.BusyToolTip),
				MessagingUtilities.GetSafeJavascriptValue(this.IdentifierRequiredMessage),
				MessagingUtilities.GetSafeJavascriptValue(this.LogOnInProgressMessage),
				MessagingUtilities.GetSafeJavascriptValue(this.AuthenticationSucceededToolTip),
				MessagingUtilities.GetSafeJavascriptValue(this.AuthenticatedAsToolTip),
				MessagingUtilities.GetSafeJavascriptValue(this.AuthenticationFailedToolTip),
				identifierParameterName,
				discoveryCallbackResultParameterName,
				discoveryErrorCallbackParameterName,
				discoveryCallback,
				Environment.NewLine);

			startupScript.AppendLine("</script>");

			Page.ClientScript.RegisterStartupScript(this.GetType(), "ajaxstartup", startupScript.ToString());
			string htmlFormat = @"
var openidbox = document.getElementsByName('{0}')[0];
if (!openidbox.dnoi_internal.onSubmit()) {{ return false; }}
";
			Page.ClientScript.RegisterOnSubmitStatement(
				this.GetType(),
				"loginvalidation",
				string.Format(CultureInfo.InvariantCulture, htmlFormat, this.Name));
		}
	}
}
