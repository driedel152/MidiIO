using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiIO
{
    public enum ProgramName
    {
        // Piano

        AcousticGrandPiano = 0,
        BrightAcousticPiano,
        ElectricGrandPiano,
        HonkytonkPiano,
        ElectricPiano1,
        ElectricPiano2,
        Harpsichord,
        Clavinet,

        // Chromatic Percussion

        Celesta,
        Glockenspiel,
        MusicBox,
        Vibraphone,
        Marimba,
        Xylophone,
        TubularBells,
        Dulcimer,

        // Organ

        DrawbarOrgan,
        PercussiveOrgan,
        RockOrgan,
        ChurchOrgan,
        ReedOrgan,
        Accordion,
        Harmonica,
        TangoAccordion,

        // Guitar

        AcousticGuitarNylon,
        AcousticGuitarSteel,
        ElectricGuitarJazz,
        ElectricGuitarClean,
        ElectricGuitarMuted,
        ElectricGuitarOverdriven,
        ElectricGuitarDistortion,
        ElectricGuitarHarmonics,

        // Bass

        AcousticBass,
        ElectricBassFinger,
        ElectricBassPicked,
        FretlessBass,
        SlapBass1,
        SlapBass2,
        SynthBass1,
        SynthBass2,

        // Strings

        Violin,
        Viola,
        Cello,
        Contrabass,
        TremoloStrings,
        PizzicatoStrings,
        OrchestralHarp,
        Timpani,

        // Ensemble

        StringEnsemble1,
        StringEnsemble2,
        SynthStrings1,
        SynthStrings2,
        ChoirAahs,
        VoiceOohs,
        SynthVoice,
        OrchestraHit,

        // Brass

        Trumpet,
        Trombone,
        Tuba,
        MutedTrumpet,
        FrenchHorn,
        BrassSection,
        SynthBrass1,
        SynthBrass2,

        // Reed

        SopranoSax,
        AltoSax,
        TenorSax,
        BaritoneSax,
        Oboe,
        EnglishHorn,
        Bassoon,
        Clarinet,

        // Pipe

        Piccolo,
        Flute,
        Recorder,
        PanFlute,
        Blownbottle,
        Shakuhachi,
        Whistle,
        Ocarina,

        // Synth Lead

        Square,
        Sawtooth,
        Calliope,
        Chiff,
        Charang,
        Spacevoice,
        Fifths,
        BassAndLead,

        // Synth Pad

        NewAge,
        Warm,
        Polysynth,
        Choir,
        BowedGlass,
        Metallic,
        Halo,
        Sweep,

        // Synth Effects

        Rain,
        Soundtrack,
        Crystal,
        Atmosphere,
        Brightness,
        Goblins,
        Echoes,
        Scifi,

        // Ethnic

        Sitar,
        Banjo,
        Shamisen,
        Koto,
        Kalimba,
        BagPipe,
        Fiddle,
        Shanai,

        // Percussive

        TinkleBell,
        Agogo,
        SteelDrums,
        Woodblock,
        TaikoDrum,
        MelodicTom,
        SynthDrum,
        ReverseCymbal,

        // Sound Effects

        GuitarFretNoise,
        BreathNoise,
        Seashore,
        BirdTweet,
        TelephoneRing,
        Helicopter,
        Applause,
        Gunshot,
    }
}
