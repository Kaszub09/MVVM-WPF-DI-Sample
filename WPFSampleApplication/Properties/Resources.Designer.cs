//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.42000
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WPFSampleApplication.Properties {
    using System;
    
    
    /// <summary>
    ///   Klasa zasobu wymagająca zdefiniowania typu do wyszukiwania zlokalizowanych ciągów itd.
    /// </summary>
    // Ta klasa została automatycznie wygenerowana za pomocą klasy StronglyTypedResourceBuilder
    // przez narzędzie, takie jak ResGen lub Visual Studio.
    // Aby dodać lub usunąć składową, edytuj plik ResX, a następnie ponownie uruchom narzędzie ResGen
    // z opcją /str lub ponownie utwórz projekt VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        /// Zwraca buforowane wystąpienie ResourceManager używane przez tę klasę.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WPFSampleApplication.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Przesłania właściwość CurrentUICulture bieżącego wątku dla wszystkich
        ///   przypadków przeszukiwania zasobów za pomocą tej klasy zasobów wymagającej zdefiniowania typu.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        /// Wyszukuje zlokalizowany ciąg podobny do ciągu &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;Languages&gt;
        ///  &lt;Language id=&quot;pol&quot; name=&quot;Polski&quot;&gt;
        ///    &lt;text id=&quot;app_name&quot;&gt;Nazwa aplikacji&lt;/text&gt;
        ///    
        ///      &lt;!--=== TOP MENU ===--&gt;
        ///    &lt;text id=&quot;top_menu_navigation&quot;&gt;Nawigacja&lt;/text&gt;
        ///    &lt;text id=&quot;top_menu_go_back&quot;&gt;Wróć&lt;/text&gt;
        ///    &lt;text id=&quot;top_menu_go_forward&quot;&gt;Naprzód&lt;/text&gt;
        ///    &lt;text id=&quot;top_menu_language&quot;&gt;Język&lt;/text&gt;
        ///    &lt;text id=&quot;top_menu_themes&quot;&gt;Motywy&lt;/text&gt;
        ///    &lt;text id=&quot;top_menu_reload&quot;&gt;Przeładuj&lt;/text&gt;
        ///    &lt;text id=&quot;top_menu_reload_languages&quot;&gt;Przeładuj języki [obcięto pozostałą część ciągu]&quot;;.
        /// </summary>
        internal static string EmbeddedLanguages {
            get {
                return ResourceManager.GetString("EmbeddedLanguages", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Wyszukuje zlokalizowany zasób typu System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap logo {
            get {
                object obj = ResourceManager.GetObject("logo", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Wyszukuje zlokalizowany zasób typu System.Drawing.Icon podobny do zasobu (Ikona).
        /// </summary>
        internal static System.Drawing.Icon trayIcon {
            get {
                object obj = ResourceManager.GetObject("trayIcon", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
    }
}
