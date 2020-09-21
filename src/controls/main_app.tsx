import * as React from 'react';
import { DrawerLayout , MainContainer} from '../components';


export const MainApp = ({
    open
}: { open?; }) => <DrawerLayout>
    <MainContainer/>
</DrawerLayout>;
