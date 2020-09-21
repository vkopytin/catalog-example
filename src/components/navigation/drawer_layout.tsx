import AppBar from '@material-ui/core/AppBar';
import Collapse from '@material-ui/core/Collapse';
import CssBaseline from '@material-ui/core/CssBaseline';
import Divider from '@material-ui/core/Divider';
import Drawer from '@material-ui/core/Drawer';
import Hidden from '@material-ui/core/Hidden';
import IconButton from '@material-ui/core/IconButton';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import { makeStyles, useTheme } from '@material-ui/core/styles';
import Tab from '@material-ui/core/Tab';
import Tabs from '@material-ui/core/Tabs';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import CloseIcon from '@material-ui/icons/Close';
import MenuIcon from '@material-ui/icons/Menu';
import RadioButtonCheckedIcon from '@material-ui/icons/RadioButtonChecked';
import RadioButtonUncheckedIcon from '@material-ui/icons/RadioButtonUnchecked';
import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { actions, selectors } from '../';


const drawerWidth = 240;
const useStyles = makeStyles(theme => ({
  root: {
    display: 'flex',
  },
  drawer: {
    [theme.breakpoints.up('sm')]: {
      width: drawerWidth,
      flexShrink: 0,
    },
  },
  appBar: {
    zIndex: theme.zIndex.drawer + 1,
  },
  menuButton: {
    marginRight: theme.spacing(2),
    [theme.breakpoints.up('sm')]: {
      display: 'none',
    },
  },
  toolbar: theme.mixins.toolbar,
  drawerPaper: {
    width: drawerWidth
  },
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
  },
  closeMenuButton: {
    marginRight: 'auto',
    marginLeft: 0,
  },
  secondaryBar: {

  }
}));

export function DrawerLayoutElement({
  manufacturers,
  children,
  expandedManufacturer,
  expandManufacturer,
  navigationTab,
  updateNavigationTab
}) {
  const classes = useStyles();
  const theme = useTheme();
  const [mobileOpen, setMobileOpen] = React.useState(false);
  function handleDrawerToggle() {
    setMobileOpen(!mobileOpen)
  }
  const listManufacturers = (
    <div>
      <List>
        {manufacturers.map(({ name = '', models = [] }, index1) => (<React.Fragment key={index1}>
          <ListItem button key={index1} onClick={e => expandManufacturer(expandedManufacturer === name ? null : name)}>
            <ListItemText primary={name} />
            {expandedManufacturer === name ? <RadioButtonCheckedIcon /> : <RadioButtonUncheckedIcon />}
          </ListItem>
          <Collapse in={false && expandedManufacturer === name} timeout="auto" unmountOnExit>
            <Divider />
            {models.length && <List component="div" disablePadding>
              {models.map(({ name: modelName = '' }, index2) => {
                return <ListItem button key={index2}>
                  <ListItemText inset primary={modelName}/>
                </ListItem>
              })}
            </List>}
          </Collapse>
        </React.Fragment>))}
      </List>
      <Divider />
    </div>
  );

  return (
    <div className={classes.root}>
      <CssBaseline />
      <AppBar position="fixed" className={classes.appBar}>
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="Open drawer"
            edge="start"
            onClick={handleDrawerToggle}
            className={classes.menuButton}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap>
            Catalog
          </Typography>
          <Tabs value={navigationTab === 'catalog' ? 0: 1} textColor="inherit">
            <Tab textColor="inherit" label="Catalog" onClick={e => updateNavigationTab('catalog')} />
            <Tab textColor="inherit" label="Favorites" onClick={e => updateNavigationTab('favorites')} />
          </Tabs>
        </Toolbar>
      </AppBar>
      
      <nav className={classes.drawer}>
        <Hidden smUp implementation="css">
          <Drawer
            variant="temporary"
            anchor={theme.direction === 'rtl' ? 'right' : 'left'}
            open={mobileOpen}
            onClose={handleDrawerToggle}
            classes={{
              paper: classes.drawerPaper,
            }}
            ModalProps={{
              keepMounted: true, // Better open performance on mobile.
            }}
          >
            <IconButton onClick={handleDrawerToggle} className={classes.closeMenuButton}>
              <CloseIcon />
            </IconButton>
            {listManufacturers}
          </Drawer>
        </Hidden>
        <Hidden xsDown implementation="css">
          <Drawer
            className={classes.drawer}
            variant="permanent"
            classes={{
              paper: classes.drawerPaper,
            }}
          >
            <div className={classes.toolbar} />
            {listManufacturers}
          </Drawer>
        </Hidden>
      </nav>
      <div className={classes.content}>
        <div className={classes.toolbar} />
        {children}
      </div>
    </div>
  );
}

const mapStateToProps = (state, props) => {
  return {
    manufacturers: selectors.navigationManufacturers(state),
    expandedManufacturer: selectors.navigationExpandedManufacturer(state),
    navigationTab: selectors.navigationTab(state),
    ...props
  };
}

const mapDispatchToProps = (dispatch, props) => {
  return {
    ...bindActionCreators(actions, dispatch),
    ...props
  };
}

const DrawerLayout = connect(mapStateToProps, mapDispatchToProps)(DrawerLayoutElement);

export { DrawerLayout };
