import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function dateOfBirthValidator(minAge: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (!value) return null;

    const dob = new Date(value);
    const today = new Date();

    if (isNaN(dob.getTime())) {
      return { invalidDate: true };
    }

    if (dob > today) {
      return { futureDate: true };
    }

    let age = today.getFullYear() - dob.getFullYear();
    const m = today.getMonth() - dob.getMonth();
    const d = today.getDate() - dob.getDate();

    if (m < 0 || (m === 0 && d < 0)) {
      age--;
    }

    return age < minAge ? { underage: { requiredAge: minAge, actualAge: age } } : null;
  };
}
