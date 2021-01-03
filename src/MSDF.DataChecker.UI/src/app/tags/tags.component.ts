import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { ToastrService } from 'ngx-toastr';
import { Tag } from '../models/tag.model';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.css'],
})
export class TagsComponent implements OnInit {
  newTag = new Tag();
  tags!: Tag[];
  filteredTags!: Tag[];
  titleButton = 'Save';
  searchInput = '';

  constructor(private apiService: ApiService, private toastr: ToastrService) {}

  ngOnInit() {
    this.setInitialValues();
    this.loadTags();
  }

  setInitialValues() {
    this.newTag.id = 0;
    this.newTag.description = '';
    this.newTag.isPublic = false;
    this.titleButton = 'Save';
    this.searchInput = '';
    this.searchTags('');
  }

  loadTags() {
    this.apiService.tag.getTags().subscribe((result) => {
      this.tags = result;
      this.searchTags('');
    });
  }

  saveTag(tagForm: FormGroup) {
    if (!tagForm.valid) {
      this.markFormGroupTouched(tagForm);
      return;
    }

    const existTagWithName = this.tags.find(
      (rec) => rec.name.toLowerCase() === this.newTag.name.toLowerCase()
    );
    if (
      existTagWithName !== undefined &&
      existTagWithName.id !== this.newTag.id
    ) {
      this.toastr.error(
        'A tag with the name ' + this.newTag.name + ' already exist.',
        'Error'
      );
      return;
    }

    if (this.newTag.id === 0) {
      this.apiService.tag.addTag(this.newTag).subscribe((result) => {
        if (result != null) {
          tagForm.reset();
          this.setInitialValues();
          this.loadTags();
          this.toastr.success('Tag created', 'Success');
        }
      });
    }
    else {
      this.apiService.tag.modifyTag(this.newTag).subscribe((result) => {
        tagForm.reset();
        this.setInitialValues();
        this.loadTags();
        this.toastr.success('Tag updated', 'Success');
      });
    }
  }

  cancelTag(tagForm: FormGroup) {
    tagForm.reset();
    this.setInitialValues();
  }

  edit(tag: any) {
    this.newTag = Object.assign({}, tag);
    this.titleButton = 'Update';
  }

  delete(tag: { id: number; }) {
    this.apiService.tag.deleteTag(tag.id).subscribe((result) => {
      this.setInitialValues();
      this.loadTags();
      this.toastr.success('Tag deleted', 'Success');
    });
  }

  searchTags(textToSearch: string | null) {
    if (textToSearch != null && textToSearch !== '') {
      this.filteredTags = this.tags.filter(
        (rec) =>
          rec.name.toLowerCase().indexOf(textToSearch) >= 0 ||
          rec.description.toLowerCase().indexOf(textToSearch) >= 0
      );
    } else {
      this.filteredTags = this.tags;
    }
  }

  private markFormGroupTouched(form: FormGroup) {
    Object.values(form.controls).forEach((control) => {
      control.markAsTouched();
      if ((control as any).controls) {
        this.markFormGroupTouched(control as FormGroup);
      }
    });
  }
}
