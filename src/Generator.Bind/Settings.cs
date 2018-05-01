/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bind
{
    [Serializable]
    internal class Settings
    {
        [Flags]
        public enum Legacy
        {
            /// <summary>
            /// Default value.
            /// </summary>
            None = 0x00,

            /// <summary>
            /// Leave enums as plain const ints.
            /// </summary>
            ConstIntEnums = 0x01,

            /// <summary>
            /// Leave enums in the default STRANGE_capitalization.ALL_CAPS form.
            /// </summary>
            NoAdvancedEnumProcessing = 0x02,

            /// <summary>
            /// Don't allow unsafe wrappers in the interface.
            /// </summary>
            NoPublicUnsafeFunctions = 0x04,

            /// <summary>
            /// Don't trim the [fdisub]v? endings from functions.
            /// </summary>
            NoTrimFunctionEnding = NoPublicUnsafeFunctions,

            /// <summary>
            /// Don't trim the [gl|wgl|glx|glu] prefixes from functions.
            /// </summary>
            NoTrimFunctionPrefix = 0x08,

            /// <summary>
            /// Don't spearate functions in different namespaces, according to their extension category
            /// (e.g. GL.Arb, GL.Ext etc).
            /// </summary>
            NoSeparateFunctionNamespaces = 0x10,

            /// <summary>
            /// No public void* parameters (should always be enabled. Disable at your own risk. Disabling
            /// means that BitmapData.Scan0 and other .Net properties/functions must be cast to (void*)
            /// explicitly, to avoid the 'object' overload from being called.)
            /// </summary>
            TurnVoidPointersToIntPtr = 0x20,

            /// <summary>
            /// Generate all possible permutations for ref/array/pointer parameters.
            /// </summary>
            GenerateAllPermutations = 0x40,

            /// <summary>
            /// Nest enums inside the GL class.
            /// </summary>
            NestedEnums = 0x80,

            /// <summary>
            /// Turn GLboolean to int (Boolean enum), not bool.
            /// </summary>
            NoBoolParameters = 0x100,

            /// <summary>
            /// Keep all enum tokens, even if same value (e.g. FooARB, FooEXT and FooSGI).
            /// </summary>
            NoDropMultipleTokens = 0x200,

            /// <summary>
            /// Do not emit inline documentation.
            /// </summary>
            NoDocumentation = 0x400,

            /// <summary>
            /// Disables ErrorHelper generation.
            /// </summary>
            NoDebugHelpers = 0x800,

            /// <summary>
            /// Generate both typed and untyped ("All") signatures for enum parameters.
            /// </summary>
            KeepUntypedEnums = 0x1000,

            /// <summary>
            /// Marks deprecated functions as [Obsolete]
            /// </summary>
            AddDeprecationWarnings = 0x2000,

            /// <summary>
            /// Use DllImport declaration for core functions (do not generate entry point slots)
            /// </summary>
            UseDllImports = 0x4000,

            /// <summary>
            /// Use in conjuction with UseDllImports, to create
            /// bindings that are compatible with opengl32.dll on Windows.
            /// This uses DllImports up to GL 1.1 and function pointers
            /// for higher versions.
            /// </summary>
            UseWindowsCompatibleGL = 0x8000,

            Tao = ConstIntEnums |
                  NoAdvancedEnumProcessing |
                  NoPublicUnsafeFunctions |
                  NoTrimFunctionEnding |
                  NoTrimFunctionPrefix |
                  NoSeparateFunctionNamespaces |
                  TurnVoidPointersToIntPtr |
                  NestedEnums |
                  NoBoolParameters |
                  NoDropMultipleTokens |
                  NoDocumentation |
                  NoDebugHelpers

            /*GenerateAllPermutations,*/
        }

        private Legacy? _compatibility;

        private string _inputPath,
            _outputPath,
            _outputNamespace,
            _docPath,
            _fallbackDocPath,
            _licenseFile,
            _languageTypeMapFile,
            _keywordEscapeCharacter,
            _importsFile,
            _delegatesFile,
            _enumsFile,
            _wrappersFile;

        /// <summary>
        /// The name of the C# enum which holds every single OpenGL enum (for compatibility purposes).
        /// </summary>
        public string CompleteEnumName = "All";

        public string ConstantPrefix = "GL_";
        public Legacy DefaultCompatibility = Legacy.NoDropMultipleTokens;
        public string DefaultDelegatesFile = "Delegates.cs";
        public string DefaultDocPath = "src/Generator.Bind/Specifications/Docs";
        public string DefaultEnumsFile = "Enums.cs";
        public string DefaultFallbackDocPath = "src/Generator.Bind/Specifications/Docs/GL";
        public string DefaultImportsFile = "Core.cs";

        public string DefaultInputPath = "src/Generator.Bind/Specifications";
        public string DefaultKeywordEscapeCharacter = "@";
        public string DefaultLanguageTypeMapFile = "csharp.tm";
        public string DefaultLicenseFile = "License.txt";
        public string DefaultOutputNamespace = "OpenTK.Graphics.OpenGL";
        public string DefaultOutputPath = "src/OpenTK/Graphics/OpenGL";
        public string DefaultWrappersFile = "GL.cs";

        public string DelegatesClass = "Delegates";
        public string EnumPrefix = "";

        // New enums namespace (don't use a nested class).
        public string EnumsNamespace = null; // = "Enums";
        public string FunctionPrefix = "gl";

        public string GLClass = "GL"; // Needed by Glu for the AuxEnumsClass. Can be set through -gl:"xxx".
        public string ImportsClass = "Core";
        public string NamespaceSeparator = ".";
        public string NestedEnumsClass = "Enums";

        // TODO: This code is too fragile.
        // Old enums code:
        public string NormalEnumsClassOverride = null;
        public string OutputClass = "GL"; // The real output class. Can be set through -class:"xxx".

        public string WindowsGdi = "OpenTK.Platform.Windows.API";

        public Settings()
        {
            OverridesFiles = new List<string>();
        }

        public string InputPath
        {
            get => _inputPath ?? DefaultInputPath;
            set => _inputPath = value;
        }

        public string OutputPath
        {
            get => _outputPath ?? DefaultOutputPath;
            set => _outputPath = value;
        }

        public string OutputNamespace
        {
            get => _outputNamespace ?? DefaultOutputNamespace;
            set => _outputNamespace = value;
        }

        public string DocPath
        {
            get => _docPath ?? DefaultDocPath;
            set => _docPath = value;
        }

        public string FallbackDocPath
        {
            get => _fallbackDocPath ?? DefaultFallbackDocPath;
            set => _fallbackDocPath = value;
        }

        public string LicenseFile
        {
            get => _licenseFile ?? DefaultLicenseFile;
            set => _licenseFile = value;
        }

        public List<string> OverridesFiles { get; }

        public string LanguageTypeMapFile
        {
            get => _languageTypeMapFile ?? DefaultLanguageTypeMapFile;
            set => _languageTypeMapFile = value;
        }

        public string KeywordEscapeCharacter
        {
            get => _keywordEscapeCharacter ?? DefaultKeywordEscapeCharacter;
            set => _keywordEscapeCharacter = value;
        }

        public string ImportsFile
        {
            get => _importsFile ?? DefaultImportsFile;
            set => _importsFile = value;
        }

        public string DelegatesFile
        {
            get => _delegatesFile ?? DefaultDelegatesFile;
            set => _delegatesFile = value;
        }

        public string EnumsFile
        {
            get => _enumsFile ?? DefaultEnumsFile;
            set => _enumsFile = value;
        }

        public string WrappersFile
        {
            get => _wrappersFile ?? DefaultWrappersFile;
            set => _wrappersFile = value;
        }

        public Legacy Compatibility
        {
            get => _compatibility ?? DefaultCompatibility;
            set => _compatibility = value;
        }

        public string NormalEnumsClass => NormalEnumsClassOverride == null
            ? string.IsNullOrEmpty(NestedEnumsClass) ? OutputClass : OutputClass + NamespaceSeparator + NestedEnumsClass
            : NormalEnumsClassOverride;

        public string AuxEnumsClass => GLClass + NamespaceSeparator + NestedEnumsClass;

        public string EnumsOutput
        {
            get
            {
                if ((Compatibility & Legacy.NestedEnums) != Legacy.None)
                {
                    return OutputNamespace + NamespaceSeparator + OutputClass + NamespaceSeparator + NestedEnumsClass;
                }

                return string.IsNullOrEmpty(EnumsNamespace)
                    ? OutputNamespace
                    : OutputNamespace + NamespaceSeparator + EnumsNamespace;
            }
        }

        public string EnumsAuxOutput
        {
            get
            {
                if ((Compatibility & Legacy.NestedEnums) != Legacy.None)
                {
                    return OutputNamespace + NamespaceSeparator + GLClass + NamespaceSeparator + NestedEnumsClass;
                }

                return OutputNamespace + NamespaceSeparator + EnumsNamespace;
            }
        }

        /// <summary>
        /// True if multiple tokens should be dropped (e.g. FooARB, FooEXT and FooSGI).
        /// </summary>
        public bool DropMultipleTokens
        {
            get => (Compatibility & Legacy.NoDropMultipleTokens) == Legacy.None;
            set
            {
                if (value)
                {
                    Compatibility |= Legacy.NoDropMultipleTokens;
                }
                else
                {
                    Compatibility &= ~Legacy.NoDropMultipleTokens;
                }
            }
        }

        // Returns true if flag is enabled.
        public bool IsEnabled(Legacy flag)
        {
            return (Compatibility & flag) != 0;
        }

        // Enables the specified flag.
        public void Enable(Legacy flag)
        {
            Compatibility |= flag;
        }

        // Disables the specified flag.
        public void Disable(Legacy flag)
        {
            Compatibility &= ~flag;
        }

        public Settings Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                return (Settings)formatter.Deserialize(stream);
            }
        }
    }
}
