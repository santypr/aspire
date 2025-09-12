export interface DragonBallCharacter {
  id: number;
  name: string;
  race: string;
  planet: string;
  transformation: string;
  technique: string;
  imageUrl?: string;
}

export interface CreateCharacterRequest {
  name: string;
  race: string;
  planet: string;
  transformation: string;
  technique: string;
  imageUrl?: string;
}

export interface UpdateCharacterRequest {
  name: string;
  race: string;
  planet: string;
  transformation: string;
  technique: string;
  imageUrl?: string;
}