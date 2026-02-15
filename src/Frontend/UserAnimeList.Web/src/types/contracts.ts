// Mirrors src/Shared/UserAnimeList.Communication (requests/responses/enums)

export enum AnimeEntryStatus {
  Watching = 0,
  Completed = 1,
  PlanToWatch = 2,
  Dropped = 3,
  OnHold = 4
}

export enum AnimeListSort {
  Name = 0,
  Score = 1,
  Progress = 2,
  Status = 3,
  Type = 4,
  DateStarted = 5,
  DateFinished = 6
}

export enum AnimeStatus {
  Airing = 0,
  Finished = 1,
  NotYetAired = 2
}

export enum AnimeType {
  Tv = 0,
  Movie = 1,
  OVA = 2,
  ONA = 3,
  Special = 4,
  Unknown = 5
}

export enum SortDirection {
  Asc = 0,
  Desc = 1
}

export enum SourceType {
  Manga = 0,
  LightNovel = 1,
  Novel = 2,
  VisualNovel = 3,
  Game = 4,
  WebNovel = 5,
  OneShot = 6,
  Doujinshi = 7,
  Original = 8,
  Unknown = 9
}

export type RequestAnimeJson = {
  name: string;
  synopsis?: string;
  episodes?: number;
  genres: string[];
  studios: string[];
  status: AnimeStatus;
  source: SourceType;
  type: AnimeType;
  airedFrom?: string;
  airedUntil?: string;
};

export type RequestAnimeListEntryFilterJson = {
  query?: string;
  status?: AnimeEntryStatus;
  dateStarted?: string;
  dateFinished?: string;
  sortField?: AnimeListSort;
  sortDirection?: SortDirection;
};

export type RequestAnimeListEntryJson = {
  animeId: string;
  status: AnimeEntryStatus;
  score?: number;
  progress?: number;
  dateStarted?: string;
  dateFinished?: string;
};

export type RequestAnimeSearchJson = { query: string };
export type RequestChangePasswordJson = { password: string; newPassword: string; confirmNewPassword: string };
export type RequestGenreGetByNameJson = { name: string };
export type RequestLoginJson = { login: string; password: string };
export type RequestNewTokenJson = { refreshToken: string };
export type RequestRegisterGenreJson = { name: string; description: string };
export type RequestRegisterStudioJson = { name: string; description: string };
export type RequestRegisterUserJson = { userName: string; email: string; password: string; confirmPassword: string };
export type RequestStudioGetByNameJson = { name: string };
export type RequestUpdateAnimeListEntryJson = {
  status: AnimeEntryStatus;
  score?: number;
  progress?: number;
  dateStarted?: string;
  dateFinished?: string;
};
export type RequestUpdateGenreJson = { name: string; description: string };
export type RequestUpdateStudioJson = { name: string; description: string };
export type RequestUpdateUserJson = { userName: string; email: string };

export type ResponseAnimeJson = {
  id: string;
  name: string;
  imageUrl: string;
  score?: number;
  synopsis?: string;
  episodes?: number;
  genres: string[];
  studios: string[];
  status: AnimeStatus;
  source: SourceType;
  type: AnimeType;
  airedFrom?: string;
  airedUntil?: string;
  premiered: string;
};

export type ResponseAnimeListEntryJson = {
  id: string;
  animeId: string;
  status: AnimeEntryStatus;
  score?: number;
  progress?: number;
  dateStarted?: string;
  dateFinished?: string;
};

export type ResponseShortAnimeListEntryJson = {
  id: string;
  name: string;
  status: AnimeEntryStatus;
  score?: number;
  progress?: number;
};

export type ResponseAnimeListsJson = { lists: ResponseShortAnimeListEntryJson[] };

export type ResponseShortAnimeJson = {
  id: string;
  name: string;
  imageUrl: string;
  score?: number;
  status: AnimeStatus;
  type: AnimeType;
  airedFrom: string;
  airedUntil: string;
};

export type ResponseAnimesJson = { animes: ResponseShortAnimeJson[] };
export type ResponseChangePasswordJson = { tokens: ResponseTokensJson };
export type ResponseErrorJson = { errors: string[]; tokenIsExpired: boolean };
export type ResponseGenreJson = { id: string; name: string; description: string };
export type ResponseGenresJson = { genres: ResponseGenreJson[] };
export type ResponseRegisteredAnimeJson = { animeId: string; name: string };
export type ResponseRegisteredGenreJson = { name: string; description: string };
export type ResponseRegisteredStudioJson = { name: string; description: string };
export type ResponseTokensJson = { accessToken: string; refreshToken: string };
export type ResponseRegisteredUserJson = { userName: string; tokens: ResponseTokensJson };
export type ResponseStudioJson = { id: string; name: string; description: string };
export type ResponseStudiosJson = { studios: ResponseStudioJson[] };
export type ResponseUpdateImageJson = { imageUrl: string };
export type ResponseUserProfileJson = { userName: string; email: string; imageUrl: string };

export type ApiResult<T> = { data: T; status: number };
