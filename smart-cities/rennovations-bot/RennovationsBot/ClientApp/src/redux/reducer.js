export default function remodelForm(state = {}, { type, payload }) {
  switch (type) {
    case "CORRECT_FORM":
      return {
        ...state,
        needsCorrections: payload.questionNumbers
      };
    case "SUBMIT_FORM":
      return {
        ...state,
        needsCorrections: [],
        isSubmitted: true
      };
    case "CLEAR_FORM":
      return {};
    case "INACTIVE_FORM":
      return {
        ...state,
        isFormInactive: true
      } 
    default:
      return state;
  }
}
