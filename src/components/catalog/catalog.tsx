import { blue, green, grey, red, yellow } from '@material-ui/core/colors';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import { withStyles } from '@material-ui/core/styles';
import Toolbar from '@material-ui/core/Toolbar';
import ArrowDownwardIcon from '@material-ui/icons/ArrowDownward';
import ArrowUpwardIcon from '@material-ui/icons/ArrowUpward';
import LayersIcon from '@material-ui/icons/Layers';
import LayersClearIcon from '@material-ui/icons/LayersClear';
import ListIcon from '@material-ui/icons/List';
import Pagination from '@material-ui/lab/Pagination';
import ToggleButton from '@material-ui/lab/ToggleButton';
import ToggleButtonGroup from '@material-ui/lab/ToggleButtonGroup';
import * as React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { actions, selectors } from '../';
import { CarsGrid, CarsList, ComboBox, Preview } from '../../controls';


const styles = theme => ({
  root: {
    flexGrow: 1
  },
  paper: {
    padding: theme.spacing(1),
    textAlign: 'center',
    color: theme.palette.text.secondary
  },
  red: {
    color: red[500]
  },
  green: {
    color: green[500]
  },
  blue: {
    color: blue[500]
  },
  black: {
    color: grey[900]
  },
  yellow: {
    color: yellow[500]
  },
  white: {
    color: grey[100]
  },
  silver: {
    color: grey[600]
  },
});

const RadioButton = withStyles(styles as any)(({ classes, colors, selectedColor, updateSelectedColor, ...props }: any) => (
  <ToggleButtonGroup
      value="left"
      exclusive
      aria-label="text alignment"
    >
    <ToggleButton
      selected={!selectedColor}
      value="none"
      title="reset color filter"
      onClick={() => updateSelectedColor()}
    ><LayersClearIcon/></ToggleButton>
    {colors.map(color => {
      return <ToggleButton
        className={classes[color]}
        selected={selectedColor === color}
        key={color}
        value={color}
        title={`filter by: ${color}`}
        onClick={() => updateSelectedColor(color)}
      ><LayersIcon/></ToggleButton>
    })}
  </ToggleButtonGroup>
));

export const CatalogElement = withStyles(styles as any)(({
  classes,
  colors,
  selectedColor,
  updateSelectedColor,
  updateSelectedCar,
  catalogSelectedPage,
  catalogTotalPages,
  updateSelectedPage,
  selectedCar,
  manufacturers,
  expandedManufacturer,
  expandManufacturer,
  hash,
  cars,
  catalogSort,
  updateCatalogSort,
  catalogDisplay,
  updateCatalogDisplay,
  updateFavoritesLike,
  updateFavoritesUnlike,
  width = 6
}: any) => (<div className={classes.root}>
  <Grid container direction="row" spacing={1}>
    <Grid item xs={12}>
      <Paper className={classes.paper}>
        <Grid container direction="row" spacing={1}>
          <Grid item md={4}>
            <ComboBox
              items={manufacturers}
              selected={hash}
              onChange={value => expandManufacturer(value)}
            />
          </Grid>
            <Grid item md={8}>
            <RadioButton row={true} colors={colors} selectedColor={selectedColor} updateSelectedColor={updateSelectedColor} />
          </Grid>
        </Grid>
      </Paper>
    </Grid>
    <Grid item xs={12}>
      <Paper className={classes.paper}>
        <Pagination
          count={catalogTotalPages}
          page={catalogSelectedPage || 1}
          shape="rounded"
          onChange={(e, page) => updateSelectedPage(page)}
        />
      </Paper>
    </Grid>
  </Grid>
  <Grid container direction="row" spacing={1}>
    <Grid item sm={12} md={4}>
      <Toolbar className={classes.toolbar}>
        <Grid
          justify="space-between" // Add it here :)
          container
          spacing={1}
        >
          <Grid item>
            <ToggleButtonGroup
              value="left"
              exclusive
              aria-label="text alignment"
            >
              <ToggleButton
                color="primary"
                selected={catalogSort === 'asc'}
                aria-label="sort - asc"
                title="sort Ascending"
                component="span"
                onClick={e => updateCatalogSort((!catalogSort || catalogSort === 'des') ? 'asc' : '')}
                value="asc">
                <ArrowDownwardIcon />
              </ToggleButton>
              <ToggleButton
                color="primary"
                selected={catalogSort === 'des'}
                aria-label="sort - desc"
                title="sort Descending"  
                component="span"
                onClick={e => updateCatalogSort((!catalogSort || catalogSort === 'asc') ? 'des' : '')}
                value="des">
                <ArrowUpwardIcon />
              </ToggleButton>
            </ToggleButtonGroup>
          </Grid>

          <Grid item>
            <ToggleButton
              color="primary"
              selected={catalogDisplay === 'list'}
              aria-label="display items as list"
              component="span"
              title={catalogDisplay === 'list' ? 'toggle Cards' : 'togle list view'}  
              onClick={e => updateCatalogDisplay(catalogDisplay === 'list' ? '' : 'list')}
              value="asc">
              <ListIcon />
            </ToggleButton>
          </Grid>
        </Grid>
      </Toolbar>
      {catalogDisplay === 'list'
        ? <CarsList
          cars={cars}
          selectedCar={selectedCar}
          updateSelectedCar={updateSelectedCar}
          updateFavoritesLike={updateFavoritesLike}
          updateFavoritesUnlike={updateFavoritesUnlike}
        />
        : <CarsGrid
          cars={cars}
          selectedCar={selectedCar}
          updateSelectedCar={updateSelectedCar}
          updateFavoritesLike={updateFavoritesLike}
          updateFavoritesUnlike={updateFavoritesUnlike}
        />}
    </Grid>
    <Grid item sm={12} md={8}>
      {selectedCar && <Preview
        item={selectedCar}
        updateFavoritesLike={updateFavoritesLike}
        updateFavoritesUnlike={updateFavoritesUnlike}
      />}
    </Grid>
  </Grid>
  <Grid container direction="row" spacing={1}>
    <Grid item xs={12}>
      <Paper className={classes.paper}>
        <Pagination
          count={catalogTotalPages}
          page={catalogSelectedPage || 1}
          shape="rounded"
          onChange={(e, page) => updateSelectedPage(page)}
        />
      </Paper>
    </Grid>
  </Grid>
</div>));

const mapStateToProps = (state, props) => {
  return {
    colors: selectors.catalogColors(state),
    selectedColor: selectors.catalogSelectedColor(state),
    catalogSelectedPage: selectors.catalogSelectedPage(state),
    catalogTotalPages: selectors.catalogTotalPages(state),
    cars: selectors.catalogListCars(state),
    selectedCar: selectors.catalogSelectedCar(state),
    manufacturers: selectors.navigationManufacturers(state),
    expandedManufacturer: selectors.navigationExpandedManufacturer(state),
    hash: selectors.navigationHash(state),
    catalogSort: selectors.catalogSort(state),
    catalogDisplay: selectors.catalogDisplay(state),
    ...props
  };
}

const mapDispatchToProps = (dispatch, props) => {
  return {
    ...bindActionCreators(actions, dispatch),
    ...props
  };
}

export const Catalog = connect(mapStateToProps, mapDispatchToProps)(CatalogElement);
