using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace Violoncello.Structurization.Editor {
   public class EntityCreationWindow : EditorWindow {
      [SerializeField] private string _name;
      [SerializeField] private string _namespace;
      [SerializeField] private bool _createInstaller;
      [SerializeField] private bool _poolable;
      [SerializeField] private List<string> _states = new();
      [SerializeField] private List<Field> _properties = new();

      private SerializedObject serializedObject;
      private VisualElement fieldsContainer;

      private string FullViewModelName => _name + "ViewModel";

      [MenuItem("Creation", menuItem = "Assets/Create/Violoncello/Entity")]
      public static void ShowExample() {
         EntityCreationWindow window = CreateWindow<EntityCreationWindow>();
         window.titleContent = new GUIContent("Entity Creator");

         window.maxSize = new(500, 600);
         window.minSize = new(350, 400);

         var settings = Resources.Load<EntityDefaultSettings>("EntityDefaultSettings");
         if (settings != null) {
            window._namespace = settings.Namespace;
            window._poolable = settings.Poolable;
            window._createInstaller = settings.CreateInstaller;
         }
      }

      public void CreateGUI() {
         serializedObject = new SerializedObject(this);

         fieldsContainer = new VisualElement();
         fieldsContainer.style.width = Length.Percent(97);
         fieldsContainer.style.marginLeft = Length.Auto();

         var toolbar = new Toolbar();
         toolbar.style.marginBottom = 10;

         rootVisualElement.Add(toolbar);

         var namespaceField = CreatePropertyField(nameof(_namespace));
         fieldsContainer.Add(namespaceField);

         var createInstallerField = CreatePropertyField(nameof(_createInstaller));
         fieldsContainer.Add(createInstallerField);

         // ViewModel

         var viewModelHeader = CreateHeader("ViewModel");
         fieldsContainer.Add(viewModelHeader);

         var nameField = CreatePropertyField(nameof(_name));
         fieldsContainer.Add(nameField);

         var poolableField = CreatePropertyField(nameof(_poolable));
         fieldsContainer.Add(poolableField);

         var propertiesField = CreatePropertyField(nameof(_properties));
         fieldsContainer.Add(propertiesField);

         // Presenter

         var presenterHeader = CreateHeader("Presenter");
         fieldsContainer.Add(presenterHeader);

         var statesField = CreatePropertyField(nameof(_states));
         fieldsContainer.Add(statesField);

         rootVisualElement.Add(fieldsContainer);

         // Bottom

         var buttonsBox = CreateBottomButtons();
         rootVisualElement.Add(buttonsBox);
      }

      private PropertyField CreatePropertyField(string name, string label = null) {
         var property = serializedObject.FindProperty(name);
         var field = new PropertyField(property) {
            label = label ?? property.displayName
         };

         field.style.maxHeight = 180;

         field.Bind(serializedObject);

         return field;
      }

      private Label CreateHeader(string text) {
         var header = new Label();

         header.text = text;
         header.style.marginTop = 10;
         header.style.marginLeft = 2.5f;
         header.style.marginBottom = 2;
         header.style.fontSize = 14;
         header.style.unityFontStyleAndWeight = FontStyle.Bold;

         return header;
      }

      private VisualElement CreateBottomButtons() {
         var buttonsBox = new VisualElement();
         buttonsBox.style.width = Length.Percent(100);
         buttonsBox.style.height = 30;
         buttonsBox.style.display = DisplayStyle.Flex;
         buttonsBox.style.flexDirection = FlexDirection.Row;
         buttonsBox.style.marginTop = Length.Auto();
         buttonsBox.style.justifyContent = Justify.FlexEnd;

         var cancelButton = new Button() {
            text = "Cancel"
         };

         cancelButton.clicked += () => CancelButtonClickedCallback();

         cancelButton.style.width = 100;
         cancelButton.style.height = 20;

         buttonsBox.Add(cancelButton);

         var createButton = new Button() {
            text = "Create"
         };

         createButton.clicked += () => CreateButtonClickedCallback();

         createButton.style.width = 100;
         createButton.style.height = 20;

         buttonsBox.Add(createButton);

         return buttonsBox;
      }

      private void CreateButtonClickedCallback() {
         if (!ValidateInputs()) {
            return;
         }

         var selection = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets).FirstOrDefault();

         if (selection == null) {
            return;
         }

         var mainFolderPath = Path.Combine(AssetDatabase.GetAssetPath(selection), _name);
         mainFolderPath.Replace('/', Path.PathSeparator);
         Directory.CreateDirectory(mainFolderPath);

         var viewModelText = CreateViewModelText();
         var viewModelPath = Path.Combine(mainFolderPath, $"{_name}ViewModel.cs");
         File.WriteAllText(viewModelPath, viewModelText);

         var presenterText = CreatePresenterText();
         var presenterPath = Path.Combine(mainFolderPath, $"{_name}Presenter.cs");
         File.WriteAllText(presenterPath, presenterText);

         var statesFolderPath = Path.Combine(mainFolderPath, "States");
         Directory.CreateDirectory(statesFolderPath);

         var statesConfigsFolderPath = Path.Combine(statesFolderPath, "Configs");
         Directory.CreateDirectory(statesConfigsFolderPath);

         foreach (var state in _states) {
            var stateText = CreateStateText(state);
            var statePath = Path.Combine(statesFolderPath, $"{_name}{state}State.cs");
            File.WriteAllText(statePath, stateText);

            var stateConfigText = CreateStateConfigText(state);
            var stateConfigPath = Path.Combine(statesConfigsFolderPath, $"{_name}{state}StateConfig.cs");
            File.WriteAllText(stateConfigPath, stateConfigText);
         }

         var installerFolderPath = Path.Combine(mainFolderPath, "Installer");
         Directory.CreateDirectory(installerFolderPath);

         var installerText = CreateInstallerText();
         var installerPath = Path.Combine(installerFolderPath, $"{_name}Installer.cs");
         File.WriteAllText(installerPath, installerText);

         AssetDatabase.Refresh();

         Close();
      }

      private string CreateViewModelText() {
         var template = Resources.Load<TextAsset>("EntityViewModel.cs");

         var text = template.text;

         var baseClass = _poolable ? "PoolableEntityViewModel" : "EntityViewModel";

         text = text.Replace("##NAMESPACE##", _namespace);
         text = text.Replace("##NAME##", _name);
         text = text.Replace("##BASECLASS##", baseClass);

         text = text.Replace("##PROPERTIES##", CreatePropertiesString());
         text = text.Replace("##METHODS##", CreateRequestMethodsString());

         text = text.Replace("##PRESENTEREVENTCALLBACKS##", CreatePresenterCallbacks());
         text = text.Replace("##BIND##", CreateBindString());
         text = text.Replace("##UNBIND##", CreateUnbindString());

         text = text.Replace("##STATEPROPERTIESREFERENCES##", CreateStatePropertiesReferencesString());
         text = text.Replace("##STATEEVENTCALLBACKS##", CreateStateCallbacks());

         return text;
      }

      private string CreatePresenterText() {
         var template = Resources.Load<TextAsset>("EntityPresenter.cs");

         var text = template.text;

         text = text.Replace("##NAMESPACE##", _namespace);
         text = text.Replace("##NAME##", _name);
         text = text.Replace("##FIELDS##", CreatePresenterFieldsString());
         text = text.Replace("##CONSTRUCTORARGUMENTS##", CreatePresenterConstructorArgumentsString());
         text = text.Replace("##CONSTRUCTORBODY##", CreatePresenterConstructorBodyString());

         return text;
      }

      private string CreateStateText(string stateName) {
         var template = Resources.Load<TextAsset>("EntityState.cs");

         var text = template.text;

         text = text.Replace("##NAMESPACE##", _namespace);
         text = text.Replace("##NAME##", _name);
         text = text.Replace("##STATENAME##", stateName);

         return text;
      }

      private string CreateStateConfigText(string stateName) {
         var template = Resources.Load<TextAsset>("EntityStateConfig.cs");

         var text = template.text;

         text = text.Replace("##NAMESPACE##", _namespace);
         text = text.Replace("##NAME##", _name);
         text = text.Replace("##STATENAME##", stateName);
         text = text.Replace("##MENUNAME##", $"Project Configuration/Entities/{_name}/States/{stateName}");

         return text;
      }

      private string CreateInstallerText() {
         var template = Resources.Load<TextAsset>("EntityInstaller.cs");

         var text = template.text;

         text = text.Replace("##NAMESPACE##", _namespace);
         text = text.Replace("##NAME##", _name);
         text = text.Replace("##MENUNAME##", $"Project Configuration/Entities/{_name}/Installer");
         text = text.Replace("##SERIALIZEDCONFIGS##", CreateSerializedConfigsInitializationString());
         text = text.Replace("##VALIDATION##", CreateValidationString());
         text = text.Replace("##INSTALLATION##", CreateInstallationString());
         text = text.Replace("##METHODS##", CreateInstallationMethodsString());

         return text;
      }

      private string CreateStatePropertiesReferencesString() {
         var str = string.Empty;

         for (int i = 0; i < _properties.Count; i++) {
            var property = _properties[i];

            var propertyName = property.GetPublicName();
            
            str += $"\t\t\tprotected {property.Type} {propertyName} {{\r\n" +
                   $"\t\t\t\tget => ViewModel.{propertyName};\r\n" +
                   $"\t\t\t\tset => ViewModel.{propertyName} = value;\r\n" +
                   $"}}";

            if (i != _properties.Count - 1) {
               str += "\r\n\r\n";
            }
         }

         return str;
      }

      private string CreatePresenterFieldsString() {
         var str = string.Empty;

         for (int i = 0; i < _states.Count; i++) {
            var state = _states[i];

            var typeName = $"{_name}{state}State.Factory";
            var fieldName = $"{InternalName(_name)}{state}StateFactory";

            str += $"\t\tprivate {typeName} {fieldName};";

            if (i != _states.Count - 1) {
               str += "\r\n";
            }
         }

         return str;
      }

      private string CreatePresenterConstructorArgumentsString() {
         var str = string.Empty;

         for (int i = 0; i < _states.Count; i++) {
            var state = _states[i];

            var typeName = $"{_name}{state}State.Factory";
            var argumentName = $"{ArgumentName(_name)}{state}StateFactory";

            str += $"\t\t\t{typeName} {argumentName}";

            if (i != _states.Count - 1) {
               str += ",";
               str += "\r\n";
            }
         }

         return str;
      }

      private string CreatePresenterConstructorBodyString() {
         var str = string.Empty;

         for (int i = 0; i < _states.Count; i++) {
            var state = _states[i];

            var fieldName = $"{InternalName(_name)}{state}StateFactory";
            var argumentName = $"{ArgumentName(_name)}{state}StateFactory";

            str += $"\t\t\t{fieldName} = {argumentName};";

            if (i != _states.Count - 1) {
               str += "\r\n";
            }
         }

         return str;
      }

      private string CreateSerializedConfigsInitializationString() {
         var str = string.Empty;

         for (int i = 0; i < _states.Count; i++) {
            var state = _states[i];

            var typeName = $"{_name}{state}StateConfig";
            var fieldName = $"{InternalName(_name)}{state}StateConfig";

            str += $"\t\t[SerializeField] private {typeName} {fieldName};";

            if (i != _states.Count - 1) {
               str += "\r\n";
            }
         }

         return str;
      }

      private string CreateValidationString() {
         var str = string.Empty;

         for (int i = 0; i < _states.Count; i++) {
            var state = _states[i];

            var fieldName = $"{InternalName(_name)}{state}StateConfig";

            str += $"\t\t\tAssert.IsNotNull({fieldName})\r\n" +
                   $"\t\t\t\t\t.Throws(\"Installation failed: {_name}{state}State Config was null.\");";

            if (i != _states.Count - 1) {
               str += "\r\n\r\n";
            }
         }

         return str;
      }

      private string CreateInstallationMethodsString() {
         var str = string.Empty;

         for (int i = 0; i < _states.Count; i++) {
            var state = _states[i];

            var stateFullName = $"{_name}{state}State";

            var configTypeName = $"{_name}{state}StateConfig";
            var configFieldName = $"{InternalName(_name)}{state}StateConfig";

            str += $"\t\tprivate void Install{state}State() {{\r\n" +
                   $"\t\t\tContainer.BindInterfacesAndSelfTo<{configTypeName}>()\r\n" +
                   $"\t\t\t\t\t\t.FromInstance({configFieldName})\r\n" +
                   $"\t\t\t\t\t\t.AsSingle();\r\n" +
                   $"\t\t\t\r\n" +
                   $"\t\t\tContainer.BindInterfacesAndSelfTo<{stateFullName}.Factory>()\r\n" +
                   $"\t\t\t\t\t\t.AsSingle();\r\n" +
                   $"\t\t}}";

            if (i != _states.Count - 1) {
               str += "\r\n\r\n";
            }
         }

         return str;
      }

      private string CreateInstallationString() {
         var str = string.Empty;

         for (int i = 0; i < _states.Count; i++) {
            var state = _states[i];

            str += $"\t\t\tInstall{state}State();";

            str += "\r\n";
         }

         str += "\r\n";

         str += $"\t\t\tContainer.Bind(typeof(EntityPresenter<{FullViewModelName}>), typeof(ITickable))\r\n" +
                $"\t\t\t\t\t\t.To<{_name}Presenter>()\r\n" +
                $"\t\t\t\t\t\t.AsSingle();";

         str += "\r\n\r\n";

         str += $"\t\t\tContainer.Bind<EntityViewModel<{FullViewModelName}>>()\r\n" +
                $"\t\t\t\t\t\t.To<{FullViewModelName}>()\r\n" +
                $"\t\t\t\t\t\t.AsSingle();";

         return str;
      }

      private string CreateBindString() {
         var bindString = string.Empty;

         for (int i = 0; i < _properties.Count; i++) {
            var property = _properties[i];

            var publicName = property.GetPublicName();

            bindString += $"\t\t\t\tviewModel.On{publicName}SetRequested += On{publicName}SetRequested;";

            if (i != _properties.Count - 1) {
               bindString += "\r\n";
            }
         }

         return bindString;
      }

      private string CreateUnbindString() {
         var bindString = string.Empty;

         for (int i = 0; i < _properties.Count; i++) {
            var property = _properties[i];

            var publicName = property.GetPublicName();

            bindString += $"\t\t\t\tviewModel.On{publicName}SetRequested -= On{publicName}SetRequested;";

            if (i != _properties.Count - 1) {
               bindString += "\r\n";
            }
         }

         return bindString;
      }

      private void CancelButtonClickedCallback() {
         Close();
      }

      private bool ValidateInputs() {
         if (string.IsNullOrEmpty(_namespace)) {
            Debug.LogWarning("Namespace field must be filled.");
            return false;
         }

         if (string.IsNullOrEmpty(_name)) {
            Debug.LogWarning("Name field must be filled.");
            return false;
         }

         if (_states.GroupBy(state => state.ToLower()).Any(group => group.Count() > 1)) {
            Debug.LogWarning("Found 2 or more states with the same name.");
            return false;
         }

         if (_properties.GroupBy(property => property.FieldName.ToLower()).Any(group => group.Count() > 1)) {
            Debug.LogWarning("Found 2 or more properties with the same name.");
            return false;
         }

         return true;
      }

      private string CreatePropertiesString() {
         var propertiesStr = string.Empty;

         for (int i = 0; i < _properties.Count; i++) {
            var property = _properties[i];

            var propertyPublicName = property.GetPublicName();

            propertiesStr += $"\t\tpublic {property.Type} {propertyPublicName} {{ get; private set; }}\r\n" +
                             $"\t\tprivate event Action<{FullViewModelName}, {property.Type}> On{propertyPublicName}SetRequested;";

            if (i != _properties.Count - 1) {
               propertiesStr += "\r\n\r\n";
            }
         }

         return propertiesStr;
      }

      private string CreateRequestMethodsString() {
         var propertiesStr = string.Empty;

         for (int i = 0; i < _properties.Count; i++) {
            var property = _properties[i];

            var propertyPublicName = property.GetPublicName();

            propertiesStr += $"\t\tpublic void Request{propertyPublicName}Set({property.Type} value) {{\r\n" +
                             $"\t\t\tOn{propertyPublicName}SetRequested?.Invoke(this, value);\r\n" +
                             $"\t\t}}";

            if (i != _properties.Count - 1) {
               propertiesStr += "\r\n\r\n";
            }
         }

         return propertiesStr;
      }

      private string CreatePresenterCallbacks() {
         var str = string.Empty;

         for (int i = 0; i < _properties.Count; i++) {
            var property = _properties[i];

            str += $"\t\t\tprotected virtual void On{property.GetPublicName()}SetRequested({FullViewModelName} viewModel, {property.Type} value) {{\r\n" +
                   $"\t\t\t\tAssert.That(ViewModelStateMachinePairs.TryGetValue(viewModel, out IStateMachine<TState> state))\r\n" +
                   $"\t\t\t\t\t\t.Throws(\"Recieved event call from an unknown ViewModel.\");\r\n" +
                   $"\t\t\t\r\n" +
                   $"\t\t\t\tstate.CurrentState.On{property.GetPublicName()}SetRequested(value);\r\n" +
                   $"\t\t\t}}";

            if (i != _properties.Count - 1) {
               str += "\r\n\r\n";
            }
         }

         return str;
      }

      private string CreateStateCallbacks() {
         var str = string.Empty;

         for (int i = 0; i < _properties.Count; i++) {
            var property = _properties[i];

            str += $"\t\t\tpublic virtual void On{property.GetPublicName()}SetRequested({property.Type} value) {{\r\n" +
                   $"\t\t\t\t\r\n" +
                   $"\t\t\t}}";

            if (i != _properties.Count - 1) {
               str += "\r\n\r\n";
            }
         }

         return str;
      }

      private string ArgumentName(string name) {
         if (string.IsNullOrEmpty(name)) {
            return string.Empty;
         }

         var internalName = name.ToCharArray();

         internalName[0] = char.ToLower(internalName[0]);

         return new string(internalName);
      }

      private string InternalName(string name) {
         if (string.IsNullOrEmpty(name)) {
            return string.Empty;
         }

         var internalName = name.ToCharArray();

         internalName[0] = char.ToLower(internalName[0]);

         return "_" + new string(internalName);
      }

      [Serializable]
      public class Field {
         [field: SerializeField] public string Type { get; set; }
         [field: SerializeField] public string FieldName { get; set; }
         
         public string GetInternalName() {
            if (string.IsNullOrEmpty(FieldName)) {
               return string.Empty;
            }

            var name = FieldName.ToCharArray();

            name[0] = char.ToLower(name[0]);

            return "_" + new string(name);
         }

         public string GetPublicName() {
            if (string.IsNullOrEmpty(FieldName)) {
               return string.Empty;
            }

            var name = FieldName.ToCharArray();

            name[0] = char.ToUpper(name[0]);

            return new string(name);
         }
      }
   }
}
