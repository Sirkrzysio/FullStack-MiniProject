import { Component } from '@angular/core';
import {TodoListComponent} from '../../todo-list/todo-list';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    TodoListComponent
  ],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {

}
