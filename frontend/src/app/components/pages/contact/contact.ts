import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContactModel} from '../../../models/contact.model';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './contact.html',
  styleUrls: ['./contact.css']
})
export class Contact {
  contact: ContactModel = { name: '', email: '', message: '' };

  sendMessage() {
    console.log(this.contact);
    alert(`Thank you, ${this.contact.name}! Your message has been received.`);
    this.contact = { name: '', email: '', message: '' }; // reset formularza
  }
}
