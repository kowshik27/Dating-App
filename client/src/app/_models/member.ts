import { Photo } from './photo';

export interface Member {
  id: number;
  username: string;
  gender: string;
  photoUrl: string;
  age: number;
  nickName: string;
  city: string;
  country: string;
  introduction: string;
  interests: string;
  lookingFor: string;
  createdAt: Date;
  lastActive: Date;
  photos: Photo[];
}
