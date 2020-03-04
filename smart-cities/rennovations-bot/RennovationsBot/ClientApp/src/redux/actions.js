export function correctForm(questionNumbers) {
  return {
    type: "CORRECT_FORM",
    payload: {
      questionNumbers
    }
  };
}

export function submitForm() {
  return {
    type: "SUBMIT_FORM"
  };
}

export function clearForm() {
  return {
    type: "CLEAR_FORM"
  };
}

export function inactivityForm() {
  return {
    type: "INACTIVE_FORM"
  };
}