// Crash sfx
game.audioEngine.SetGlobalVariable("crashVolume", car.velocity.Length() / car.maxSpeed);
carCrash = game.soundBank.GetCue("crash");
carCrash.Play();