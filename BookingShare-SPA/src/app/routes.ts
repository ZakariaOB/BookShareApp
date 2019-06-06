import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member-list/member-list.component';
import { MessageComponent } from './message/message.component';
import { ListsComponent } from './lists/lists.component';


export const appRoutes: Routes = [
    {path: 'home', component: HomeComponent},
    {path: 'members', component: MemberListComponent},
    {path: 'messages', component: MessageComponent},
    {path: 'lists', component: ListsComponent},
    {path: '**', redirectTo: 'home', pathMatch:'full'}
];
