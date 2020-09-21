import Card from "@material-ui/core/Card";
import CardContent from "@material-ui/core/CardContent";
import CardHeader from '@material-ui/core/CardHeader';
import CardMedia from "@material-ui/core/CardMedia";
import IconButton from '@material-ui/core/IconButton';
import { withStyles } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import FavoriteIcon from '@material-ui/icons/Favorite';
import FavoriteIconBorder from '@material-ui/icons/FavoriteBorder';
import React from "react";


const styles = theme => ({
  card: {
    //maxWidth: 600,
    margin: 'auto',
    transition: '0.3s',
    boxShadow: '0 8px 40px -12px rgba(0,0,0,0.3)',
    '&:hover': {
      boxShadow: "0 16px 70px -12.125px rgba(0,0,0,0.3)"
    }
  },
  media: {
      paddingTop: '56.25%',
      'background-size': 'contain',
      'background-repeat': 'no-repeat',
      'background-position': 'center'
  },
  content: {
    textAlign: 'left',
    padding: theme.spacing(3)
  },
  divider: {
    margin: `${theme.spacing(3)}px 0`
  },
  heading: {
    fontWeight: 'bold'
  },
  subheading: {
    lineHeight: 1.8
  },
  avatar: {
    display: 'inline-block',
    border: '2px solid white',
    '&:not(:first-of-type)': {
      marginLeft: -theme.spacing()
    }
  },
  toolbar: {
    'justify-content': 'center'
  }
});

const CardPreviewElement = ({
    classes,
    item,
    updateFavoritesUnlike,
    updateFavoritesLike
}) => <Card className={classes.card}>
    <CardHeader
        title={`${item.modelName}: ${item.manufacturerName}`}
        subheader={`${item?.mileage?.number} ${item?.mileage?.unit}`}
        action={
            <IconButton aria-label="settings" onClick={e => item.isLiked
                ? updateFavoritesUnlike(item)
                : updateFavoritesLike(item)}>
                {item.isLiked ? <FavoriteIcon /> : <FavoriteIconBorder />}
            </IconButton>
          }
      />
    <CardMedia
        className={classes.media}
        image={item.pictureUrl}
    />
    <CardContent className={classes.content}>
        <Typography
            className={"MuiTypography--subheading"}
            variant={"caption"}
        >
            <div>color: {item.color}</div>
            <div>fuel: {item.fuelType}</div>
            <div>manufactured by: {item.manufacturerName}</div>
            <div>milage: {item?.mileage?.number}&nbsp;{item?.mileage?.unit}</div>
            <div>model: {item.modelName}</div>
        </Typography>
    </CardContent>
</Card>;

export const Preview = withStyles(styles as any)(CardPreviewElement);
