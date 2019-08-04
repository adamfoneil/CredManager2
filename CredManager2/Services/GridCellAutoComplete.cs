using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CredManager2.Services
{
    /// <summary>
    /// adapted from https://www.c-sharpcorner.com/UploadFile/c5c6e2/datagridview-autocomplete-textbox/
    /// </summary>
    public class GridCellAutoComplete
    {        
        private readonly DataGridView _dataGridView;
        private readonly int _colIndex;
        private readonly AutoCompleteStringCollection _items;

        private bool _showControl = false;

        public GridCellAutoComplete(DataGridViewTextBoxColumn column, IEnumerable<string> items)
        {
            _colIndex = column.Index;
            _dataGridView = column.DataGridView;            
            _dataGridView.CellEnter += CellEnter;
            _dataGridView.EditingControlShowing += EditControlShowing;

            _items = new AutoCompleteStringCollection();
            _items.AddRange(items.ToArray());            
        }

        private void CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            _showControl = (e.ColumnIndex == _colIndex);
        }

        private void EditControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (_showControl)
            {
                var textBox = e.Control as TextBox;
                textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                textBox.AutoCompleteCustomSource = _items;                
            }
        }
    }
}
