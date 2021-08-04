using SimpleClassicThemeTaskbar.Helpers;

using System;
using System.Linq;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.Forms
{
    public partial class ProgramFilterSettings : Form
    {
        private const int PROCESS_NAME_INDEX = 0;
        private const int WINDOW_CLASS_INDEX = 1;
        private const int WINDOW_TITLE_INDEX = 2;

        public ProgramFilterSettings()
        {
            InitializeComponent();
        }

        private void ProgramFilterSettings_Load(object sender, EventArgs e)
        {
            LoadItems();
            typeComboBox.SelectedIndex = 0;
        }

        private void LoadItems()
        {
            listView.Items.Clear();

            //Load filters
            foreach (string filter in Filters)
            {
                if (filter == "")
                    continue;

                string[] filterParts = filter.Split('|');

                var lvi = new ListViewItem(filterParts)
                {
                    Tag = filter,
                    ImageKey = filterParts[1],
                };

                listView.Items.Add(lvi);
            }

            foreach (ColumnHeader column in listView.Columns)
            {
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var type = typeComboBox.SelectedIndex switch
            {
                PROCESS_NAME_INDEX => "ProcessName",
                WINDOW_CLASS_INDEX => "ClassName",
                WINDOW_TITLE_INDEX => "WindowName",
                _ => throw new NotImplementedException()
            };

            var item = $"{valueTextBox.Text}|{type}";

            var filters = Filters;

            if (filters.Contains(item))
            {
                MessageBox.Show(this, "An item with the same name already exists.");
            }
            else
            {
                Filters = filters.Append(item).ToArray();
                LoadItems();
            }
        }

        private static string[] Filters
        {
            get => Config.Default.TaskbarProgramFilter.Split('*');
            set => Config.Default.TaskbarProgramFilter = string.Join('*', value);
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            var selectedItems = listView.SelectedItems.Cast<ListViewItem>();
            Filters = Filters.Where(v => !selectedItems.Any(i => (i.Tag as string) == v)).ToArray();
            LoadItems();
        }

        private void ValueTextBox_TextChanged(object sender, EventArgs e)
        {
            addButton.Enabled = !string.IsNullOrWhiteSpace(valueTextBox.Text);
        }

        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeButton.Enabled = listView.SelectedItems.Count != 0;
        }

        private void ValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddButton_Click(null, EventArgs.Empty);

                valueTextBox.Clear();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
