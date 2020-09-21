import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import CardHeader from '@material-ui/core/CardHeader';
import CardMedia from '@material-ui/core/CardMedia';
import Divider from '@material-ui/core/Divider';
import Grid from '@material-ui/core/Grid';
import IconButton from '@material-ui/core/IconButton';
import Paper from '@material-ui/core/Paper';
import { withStyles } from '@material-ui/core/styles';
import TextField from '@material-ui/core/TextField';
import Typography from '@material-ui/core/Typography';
import FavoriteIcon from '@material-ui/icons/Favorite';
import FavoriteIconBorder from '@material-ui/icons/FavoriteBorder';
import Autocomplete from '@material-ui/lab/Autocomplete';
import * as React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { actions, selectors } from '../';


export const ComboBox = ({ items, selected, onChange }) => <Autocomplete
  id="combo-box-demo"
  options={items}
  autoComplete={true}
  getOptionLabel={(option: any) => '' +  option}
  style={{ width: 300 }}
  value={items.filter(item => item === selected)[0] || ''}
  onChange={(e, newValue: any) => onChange(newValue)}
  renderInput={(params) => <TextField {...params} label="Manufacturers" variant="outlined" />}
/>;

const styles = theme => ({
    root: {
    },
    paper: {
        padding: theme.spacing(1),
        textAlign: 'center',
        color: theme.palette.text.secondary
    },
    card: {
        maxWidth: 600,
        margin: '0 2px',
        transition: '0.3s',
        boxShadow: '0 8px 40px -12px rgba(0,0,0,0.3)',
        '&:hover': {
            boxShadow: '0 16px 70px -12.125px rgba(0,0,0,0.3)'
        }
    },
    toolbar: {
        'justify-content': 'center'
    },
    content: {
        textAlign: 'left',
        padding: theme.spacing(1),
        '&:last-child': {
            paddingBottom: '4px'
        }
    },
    divider: {
        margin: `${theme.spacing(3)}px 0`
    },
    media: {
        paddingTop: '48.25%',
        margin: theme.spacing(1)
    },
});

export const FavoritestElement = withStyles(styles as any)(({
    classes,
    updateFavoritesUnlike,
    updateFavoritesLike,
    cars,
    suggestions,
    selectedSuggestion,
    updateFavoritesSelectedSuggestion
}: any) => <>
    <Grid container className={classes.root} spacing={1}>
        <Grid item xs={12}>
            <Paper className={classes.paper}>
                <ComboBox items={suggestions} selected={selectedSuggestion} onChange={suggestion => updateFavoritesSelectedSuggestion(suggestion)}/>
            </Paper>
        </Grid>
        {cars.map((item, index) => <Grid item key={index} xs={6} md={3}>
            <Card className={classes.card}>
                <CardHeader
                    title={`${item.modelName}`}
                    subheader={`${item.manufacturerName} - ${item.color}`}
                    action={
                        <IconButton aria-label="settings" onClick={e => item.isLiked
                            ? updateFavoritesUnlike(item)
                            : updateFavoritesLike(item)}>
                            {item.isLiked ? <FavoriteIcon /> : <FavoriteIconBorder />}
                        </IconButton>
                    }
                />
                {item.pictureUrl && <CardMedia
                    className={classes.media}
                    image={item.pictureUrl}
                />}
                <CardContent className={classes.content}>
                    <Typography
                        className={"MuiTypography--subheading"}
                        variant={"caption"}
                    >
                        {item?.mileage?.number} {item?.mileage?.unit}
                        <Divider />
                fuel:&nbsp;{item.fuelType}
                    </Typography>
                </CardContent>
            </Card>
        </Grid>)}
    </Grid></>);

const mapPropsToState = (state, props) => {
    return {
        cars: selectors.favoritesList(state),
        suggestions: selectors.favoritesSuggestions(state),
        selectedSuggestion: selectors.favoritesSelectedSuggestion(state),
        ...props
    };
};

const mapDispatchToProps = (dispatch, props) => {
    return {
        ...bindActionCreators(actions, dispatch),
        ...props
    };
};

export const Favorites = connect(mapPropsToState, mapDispatchToProps)(FavoritestElement);
