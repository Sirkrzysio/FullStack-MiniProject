import { Pipe, PipeTransform } from '@angular/core';
import { TodoItem } from '../models/todo-item';

@Pipe({
  name: 'filterTodos',
  standalone: true
})
export class FilterTodosPipe implements PipeTransform {
  transform(todos: TodoItem[], filter: 'all' | 'active' | 'completed'): TodoItem[] {
    if (filter === 'all') return todos;
    if (filter === 'active') return todos.filter(t => !t.completed);
    if (filter === 'completed') return todos.filter(t => t.completed);
    return todos;
  }
}
