import * as React from 'react';
import { withStyles } from '@material-ui/core/styles';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemSecondaryAction from '@material-ui/core/ListItemSecondaryAction';
import ListItemText from '@material-ui/core/ListItemText';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import Typography from '@material-ui/core/Typography';
import FavoriteIcon from '@material-ui/icons/Favorite';
import FavoriteIconBorder from '@material-ui/icons/FavoriteBorder';
import IconButton from '@material-ui/core/IconButton';


const lazyround = (num) => {
  // Nine Zeroes for Billions
  return Math.abs(Number(num)) >= 1.0e+9
    ? Math.round(Math.abs(Number(num)) / 1.0e+9) + "B"
    // Six Zeroes for Millions
    : Math.abs(Number(num)) >= 1.0e+6
      ? Math.round(Math.abs(Number(num)) / 1.0e+6) + "M"
      // Three Zeroes for Thousands
      : Math.abs(Number(num)) >= 1.0e+3
        ? Math.round(Math.abs(Number(num)) / 1.0e+3) + "K"
        : Math.abs(Number(num));
};

const styles = theme => ({
    root: {
      flexGrow: 1
    },
    paper: {
      padding: theme.spacing(1),
      textAlign: 'center',
      color: theme.palette.text.secondary
    }
});

export const CarsList = withStyles(styles as any)(({
  classes,
  cars,
  selectedCar,
  updateSelectedCar,
  updateFavoritesLike,
  updateFavoritesUnlike
}: {
  classes?;
  cars;
  selectedCar;
  updateSelectedCar;
  updateFavoritesLike;
  updateFavoritesUnlike;
}) => <List dense className={classes.root}>
    {cars.map((item, index) => <ListItem
      key={item.stockNumber}
      button
      onClick={e => updateSelectedCar(item)}
      selected={selectedCar?.stockNumber === item.stockNumber}
    >
    <ListItemIcon>
      <img
        alt={item.modelName}
        src={item.pictureUrl}
        width={32}
      />
    </ListItemIcon>
    <ListItemText id={index} secondary={
      <React.Fragment>
        <Typography component="span" variant="body1">
          {item.modelName}/{item.manufacturerName} - {lazyround(item?.mileage?.number)}&nbsp;{item?.mileage?.unit}
        </Typography>
        <br />
        <Typography
          component="span"
          variant="body2"
          className={classes.inline}
          color="textPrimary"
        >
          fuel:&nbsp;{item.fuelType} - {item.color}
        </Typography>
      </React.Fragment>
    } />
    <ListItemSecondaryAction>
      <IconButton value="liked" onClick={e => item.isLiked
        ? updateFavoritesUnlike(item)
        : updateFavoritesLike(item)}>
          {item.isLiked ? <FavoriteIcon /> : <FavoriteIconBorder />}
      </IconButton>
    </ListItemSecondaryAction>
  </ListItem>)}
</List>);
