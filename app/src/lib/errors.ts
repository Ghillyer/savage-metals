import { isAxiosError } from 'axios';

interface ProblemDetails {
  title?: string;
  detail?: string;
}

export function getErrorMessage(error: unknown) {
  if (isAxiosError<ProblemDetails>(error)) {
    return error.response?.data?.detail ?? error.response?.data?.title ?? error.message;
  }

  return 'Something went wrong. Please try again.';
}