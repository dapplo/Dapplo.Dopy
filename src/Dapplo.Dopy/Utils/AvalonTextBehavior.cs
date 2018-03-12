using System;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;

namespace Dapplo.Dopy.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AvalonEditBehaviour : Behavior<TextEditor>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty GiveMeTheTextProperty = DependencyProperty.Register("GiveMeTheText", typeof(string), typeof(AvalonEditBehaviour), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        /// <summary>
        /// 
        /// </summary>
        public string GiveMeTheText
        {
            get => (string)GetValue(GiveMeTheTextProperty);
            set => SetValue(GiveMeTheTextProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = sender as TextEditor;
            if (textEditor?.Document != null) GiveMeTheText = textEditor.Document.Text; 
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as AvalonEditBehaviour;
            var editor = behavior?.AssociatedObject;
            if (editor?.Document == null)
            {
                return;
            }
            var caretOffset = editor.CaretOffset;
            editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
            editor.CaretOffset = caretOffset;
        }
    }
}
