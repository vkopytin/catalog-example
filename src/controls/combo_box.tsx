import * as React from 'react';
import TextField from '@material-ui/core/TextField';
import Autocomplete from '@material-ui/lab/Autocomplete';


export const ComboBox = ({ items, selected, onChange }) => <Autocomplete
  id="combo-box-demo"
  options={items}
  autoComplete={true}
  getOptionLabel={(option: any) => option.name}
  style={{ width: 300 }}
  value={items.filter(item => item.name === selected)[0] || {name: ''}}
  onChange={(e, newValue: any) => onChange(newValue ? newValue.name : '')}
  renderInput={(params) => <TextField {...params} label="Manufacturers" variant="outlined" />}
/>;
