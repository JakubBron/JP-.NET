#include <windows.h>
using namespace System;

using namespace System::Reflection;
using namespace System::Security;
using namespace System::Runtime::Remoting;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;
using namespace System::ComponentModel;
using namespace System::Windows::Input;

typedef ObservableCollection<MethodInfo^> myList;

ref class DataModel : INotifyPropertyChanged {

#pragma region Private Fields

	String^ _codeText;
	ICollection<String^>^ _errorsList = gcnew ObservableCollection<String^>();
	ICollection<String^>^ _methodsList = gcnew ObservableCollection<String^>();
	ICollection<String^>^ _fieldsList = gcnew ObservableCollection<String^>();
	int outputTypeTarget = 1; 
	array<MethodInfo^>^ methods = nullptr;
	array<FieldInfo^>^ fields = nullptr;
	Object^ objectInstance;
	System::Type^ type;
	ObjectHandle^ handle;
	ICommand^ _AddCodeCommand;
	ICommand^ _InvokeMethodCommand;
	ICommand^ _SetFieldCommand;
	String^ _methodParameter;
	String^ _methodResult;
	String^ _fieldValue;
	String^ _fieldType;
	String^ _selectedMethod;
	String^ _selectedField;

#pragma endregion

public:

#pragma region Public Constructors

	DataModel(String^ codeText);

#pragma endregion

#pragma region Public Events

	virtual event System::ComponentModel::PropertyChangedEventHandler^ PropertyChanged;

#pragma endregion

#pragma region Public Properties

	property String^ CodeText {
		String^ get() {
			return _codeText;
		}

		void set(String^ value) {
			if (_codeText != value) {
				_codeText = value;
				OnPropertyChanged("CodeText");
			}
		}
	}

	property ICollection<String^>^ ErrorsList {
		ICollection<String^>^ get() {
			return _errorsList;
		}

		void set(ICollection<String^>^ value) {
			_errorsList = value;
		}
	}

	property ICollection<String^>^ MethodsList {
		ICollection<String^>^ get() {
			return _methodsList;
		}

		void set(ICollection<String^>^ value) {
			_methodsList = value;
		}
	}

	property ICollection<String^>^ FieldsList {
		ICollection<String^>^ get() {
			return _fieldsList;
		}

		void set(ICollection<String^>^ value) {
			_fieldsList = value;
		}
	}

	property String^ MethodParameter {
		String^ get() {
			return _methodParameter;
		}

		void set(String^ value) {
			_methodParameter = value;
		}
	}

	property String^ MethodResult {
		String^ get() {
			return _methodResult;
		}

		void set(String^ value) {
			if (_methodResult != value) {
				_methodResult = value;
				OnPropertyChanged("MethodResult");
			}
		}
	}

	property String^ FieldValue {
		String^ get() {
			return _fieldValue;
		}

		void set(String^ value) {
			if (_fieldValue != value) {
				_fieldValue = value;
				OnPropertyChanged("FieldValue");
			}
		}
	}

	property String^ FieldType {
		String^ get() {
			return _fieldType;
		}

		void set(String^ value) {
			if (_fieldType != value) {
				_fieldType = value;
				OnPropertyChanged("FieldType");
			}
		}
	}

	property String^ SelectedMethod {
		String^ get() {
			return _selectedMethod;
		}

		void set(String^ value) {
			_selectedMethod = value;
		}
	}

	property String^ SelectedField {
		String^ get() {
			return _selectedField;
		}

		void set(String^ value) {
			_selectedField = value;
			UpdateFieldInfo();
		}
	}

	property ICommand^ AddCodeCommand {
		ICommand^ get() {
			if (_AddCodeCommand == nullptr)
			{
				_AddCodeCommand = gcnew NavigateToAddCodeCommand(this);
			}
			return _AddCodeCommand;
		}
		void set(ICommand^ value) {
			_AddCodeCommand = value;
		}
	}

	property ICommand^ InvokeMethodCommand {
		ICommand^ get() {
			if (_InvokeMethodCommand == nullptr)
			{
				_InvokeMethodCommand = gcnew InvokeMethodCommandClass(this);
			}
			return _InvokeMethodCommand;
		}
		void set(ICommand^ value) {
			_InvokeMethodCommand = value;
		}
	}

	property ICommand^ SetFieldCommand {
		ICommand^ get() {
			if (_SetFieldCommand == nullptr)
			{
				_SetFieldCommand = gcnew SetFieldCommandClass(this);
			}
			return _SetFieldCommand;
		}
		void set(ICommand^ value) {
			_SetFieldCommand = value;
		}
	}

	property int OutputTypeTarget {
		int get() {
			return outputTypeTarget;
		}

		void set(int value) {
			outputTypeTarget = value;
		}
	}

	property Object^ OutputTypeTargetObject {
		Object^ get() {
			return (Microsoft::CodeAnalysis::OutputKind)outputTypeTarget;
		}

		void set(Object^ value) {
			if (value != nullptr) {
				outputTypeTarget = (int)value;
			}
		}
	}
	
	property array<MethodInfo^>^ Methods {
		array<MethodInfo^>^ get() {
			return methods;
		}

		void set(array<MethodInfo^>^ value) {
			methods = value;
			UpdateMethodsList();
		}
	}

	property array<FieldInfo^>^ Fields {
		array<FieldInfo^>^ get() {
			return fields;
		}

		void set(array<FieldInfo^>^ value) {
			fields = value;
			UpdateFieldsList();
		}
	}

	property Object^ ObjectInstance {
		Object^ get() {
			return objectInstance;
		}

		void set(Object^ value) {
			objectInstance = value;
		}
	}

	property ObjectHandle^ Handle {
		ObjectHandle^ get() {
			return handle;
		}

		void set(ObjectHandle^ value) {
			handle = value;
		}
	}

	property System::Type^ Type {
		System::Type^ get() {
			return type;
		}

		void set(System::Type^ value) {
			type = value;
		}
	}

#pragma endregion

#pragma region Private Methods

	void UpdateMethodsList();
	void UpdateFieldsList();
	void UpdateFieldInfo();
	
	void OnPropertyChanged(String^ propertyName) {
		PropertyChanged(this, gcnew PropertyChangedEventArgs(propertyName));
	}

#pragma endregion

#pragma region Public Nested Classes

	ref class NavigateToAddCodeCommand : public ICommand {

		DataModel^ _viewModel;
		
		property DataModel^ ViewModel {
			DataModel^ get() {
				return _viewModel;
			}
		
			void set(DataModel^ value) {
				_viewModel = value;
			}
		}
		virtual bool CanExecute(System::Object^ parameter) = ICommand::CanExecute;
		virtual void Execute(System::Object^ parameter) = ICommand::Execute;
	public:

		NavigateToAddCodeCommand(DataModel^ viewModel);
		virtual event EventHandler^ CanExecuteChanged {
			void add(EventHandler^) {}
			void remove(EventHandler^) {}
		}
	};

	ref class InvokeMethodCommandClass : public ICommand {
		DataModel^ _viewModel;
		
	public:
		InvokeMethodCommandClass(DataModel^ viewModel);
		virtual bool CanExecute(System::Object^ parameter);
		virtual void Execute(System::Object^ parameter);
		virtual event EventHandler^ CanExecuteChanged {
			void add(EventHandler^) {}
			void remove(EventHandler^) {}
		}
	};

	ref class SetFieldCommandClass : public ICommand {
		DataModel^ _viewModel;
		
	public:
		SetFieldCommandClass(DataModel^ viewModel);
		virtual bool CanExecute(System::Object^ parameter);
		virtual void Execute(System::Object^ parameter);
		virtual event EventHandler^ CanExecuteChanged {
			void add(EventHandler^) {}
			void remove(EventHandler^) {}
		}
	};

#pragma endregion

};

