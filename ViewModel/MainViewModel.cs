using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Dictionary.Table;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using Dictionary.Database;
using MySql.Data.MySqlClient;

namespace Dictionary.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        DictionaryEntries = new ObservableCollection<DictionaryEntry>();
        LoadEntries();
    }
    
    // binding properties with the UI
    [ObservableProperty]
    private string termText;
    [ObservableProperty]
    private string definitionText;
    [ObservableProperty]
    private string synonymText;
    [ObservableProperty]
    private DictionaryEntry? selectedDictionaryEntry;
    [ObservableProperty]
    private ObservableCollection<DictionaryEntry> dictionaryEntries;


    // add button
    [RelayCommand]
    public void Add()
    {
        if (string.IsNullOrWhiteSpace(TermText)
            || string.IsNullOrWhiteSpace(DefinitionText)
            || string.IsNullOrWhiteSpace(SynonymText))
        {
            MessageBox.Show("Please fill all fields.");
            return;
        }

        string term = TermText;
        string definition = DefinitionText;
        string synonym = SynonymText;

        string queryAdd = $@"insert into dictionary_entry (Term, DefinitionTerm, Synonym)
                            values (@Term, @DefinitionTerm, @Synonym)";

        using (var connection = DictionaryDb.GetConnection())
        {
            connection.Open();

            using (MySqlCommand cmd = new MySqlCommand(queryAdd, connection))
            {
                cmd.Parameters.AddWithValue("@Term", term);
                cmd.Parameters.AddWithValue("@DefinitionTerm", definition);
                cmd.Parameters.AddWithValue("@Synonym", synonym);
                cmd.ExecuteNonQuery();
            }
        }

        MessageBox.Show("Entry added.");
        ClearFields();

        DictionaryEntries.Add(new DictionaryEntry
        {
            Term = term,
            Definition = definition,
            Synonym = synonym
        });
    }


    // delete button
    [RelayCommand]
    public void Delete()
    {
        if (string.IsNullOrWhiteSpace(TermText)
            || string.IsNullOrWhiteSpace(DefinitionText)
            || string.IsNullOrWhiteSpace(SynonymText))
        {
            MessageBox.Show("Please choose an entry.");
            return;
        }

        int id = SelectedDictionaryEntry.Id;

        string queryDelete = $@"delete from dictionary_entry where Id = @Id";

        using (var connection = DictionaryDb.GetConnection())
        {
            connection.Open();

            using (MySqlCommand cmd = new MySqlCommand(queryDelete, connection))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        DictionaryEntries.Remove(SelectedDictionaryEntry);
        MessageBox.Show("Entry deleted.");
        ClearFields();
    }

    //clear button
    [RelayCommand]
    public void Clear()
    {
        ClearFields();
    }

    // filling and loading of entries into DataGrid
    public void LoadEntries ()
    {
        var list = new ObservableCollection<DictionaryEntry>();
        using (var connection = DictionaryDb.GetConnection())
        {
            connection.Open();

            var querySelect = $@"select Id, Term, DefinitionTerm, Synonym from dictionary_entry";

            using (MySqlCommand cmd = new MySqlCommand(querySelect, connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new DictionaryEntry
                    {
                        Id = reader.GetInt32(0),
                        Term = reader.GetString(1),
                        Definition = reader.GetString(2),
                        Synonym = reader.GetString(3)
                    });
                }
            }            
        }
        DictionaryEntries = list;
    }

    // fills TextBoxes when entry is selected
    partial void OnSelectedDictionaryEntryChanged(DictionaryEntry? value)
    {
        if (value is not  null)
        {
            TermText = value.Term;
            DefinitionText = value.Definition;
            SynonymText = value.Synonym;
        }
        else
        {
            TermText = string.Empty; 
            DefinitionText = string.Empty;
            SynonymText = string.Empty;
        }
    }

    // clears all TextBoxes
    public void ClearFields()
    {
        TermText = string.Empty;
        DefinitionText = string.Empty;
        SynonymText = string.Empty;
    }
}