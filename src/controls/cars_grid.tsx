import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import CardHeader from '@material-ui/core/CardHeader';
import CardMedia from '@material-ui/core/CardMedia';
import Divider from '@material-ui/core/Divider';
import Grid from '@material-ui/core/Grid';
import IconButton from '@material-ui/core/IconButton';
import { withStyles } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import FavoriteIcon from '@material-ui/icons/Favorite';
import FavoriteIconBorder from '@material-ui/icons/FavoriteBorder';
import * as React from 'react';


const styles = theme => ({
    root: {
    },
    card: {
        maxWidth: 600,
        margin: '0 2px',
        padding: '1px',
        transition: '0.3s',
        boxShadow: '0 8px 40px -12px rgba(0,0,0,0.3)',
        '&:hover': {
            boxShadow: '0 16px 70px -12.125px rgba(0,0,0,0.3)'
        }
    },
    cardSelected: {
        maxWidth: 600,
        margin: '0 2px',
        transition: '0.3s',
        border: '1px solid grey',
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
        paddingTop: '56.25%',
        'background-size': 'contain',
        'background-repeat': 'no-repeat',
        'background-position': 'center'
    },
});

export const CarsGrid = withStyles(styles as any)(({
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
}) => <Grid container className={classes.root} spacing={1}>
        {cars.map((item, index) => <Grid item key={item.stockNumber} xs={6}>
            <Card
                className={selectedCar?.stockNumber === item.stockNumber ? classes.cardSelected : classes.card}
                onClick={e => updateSelectedCar(item)}
            >
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
                <CardMedia
                    className={classes.media}
                    image={item.pictureUrl}
                />
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
    </Grid>);
