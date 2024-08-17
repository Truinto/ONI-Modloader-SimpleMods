
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared
{
    [JsonConverter(typeof(Converter))]
    public readonly struct LanguageCodes
    {
        public readonly int Int;

        public LanguageCodes(int code)
        {
            this.Int = code;
        }

        public readonly override string ToString()
        {
            return this.Int switch
            {
                0 => "",
                und => "und",
                abk => "abk",
                aar => "aar",
                afr => "afr",
                aka => "aka",
                sqi => "sqi",
                amh => "amh",
                ara => "ara",
                arg => "arg",
                hye => "hye",
                asm => "asm",
                ava => "ava",
                ave => "ave",
                aym => "aym",
                aze => "aze",
                bam => "bam",
                bak => "bak",
                eus => "eus",
                bel => "bel",
                ben => "ben",
                bis => "bis",
                bos => "bos",
                bre => "bre",
                bul => "bul",
                mya => "mya",
                cat => "cat",
                cha => "cha",
                che => "che",
                nya => "nya",
                zho => "zho",
                chu => "chu",
                chv => "chv",
                cor => "cor",
                cos => "cos",
                cre => "cre",
                hrv => "hrv",
                ces => "ces",
                dan => "dan",
                div => "div",
                nld => "nld",
                dzo => "dzo",
                eng => "eng",
                epo => "epo",
                est => "est",
                ewe => "ewe",
                fao => "fao",
                fij => "fij",
                fin => "fin",
                fra => "fra",
                fry => "fry",
                ful => "ful",
                gla => "gla",
                glg => "glg",
                lug => "lug",
                kat => "kat",
                deu => "deu",
                ell => "ell",
                kal => "kal",
                grn => "grn",
                guj => "guj",
                hat => "hat",
                hau => "hau",
                heb => "heb",
                her => "her",
                hin => "hin",
                hmo => "hmo",
                hun => "hun",
                isl => "isl",
                ido => "ido",
                ibo => "ibo",
                ind => "ind",
                ina => "ina",
                ile => "ile",
                iku => "iku",
                ipk => "ipk",
                gle => "gle",
                ita => "ita",
                jpn => "jpn",
                jav => "jav",
                kan => "kan",
                kau => "kau",
                kas => "kas",
                kaz => "kaz",
                khm => "khm",
                kik => "kik",
                kin => "kin",
                kir => "kir",
                kom => "kom",
                kon => "kon",
                kor => "kor",
                kua => "kua",
                kur => "kur",
                lao => "lao",
                lat => "lat",
                lav => "lav",
                lim => "lim",
                lin => "lin",
                lit => "lit",
                lub => "lub",
                ltz => "ltz",
                mkd => "mkd",
                mlg => "mlg",
                msa => "msa",
                mal => "mal",
                mlt => "mlt",
                glv => "glv",
                mri => "mri",
                mar => "mar",
                mah => "mah",
                mon => "mon",
                nau => "nau",
                nav => "nav",
                nde => "nde",
                nbl => "nbl",
                ndo => "ndo",
                nep => "nep",
                nor => "nor",
                nob => "nob",
                nno => "nno",
                oci => "oci",
                oji => "oji",
                ori => "ori",
                orm => "orm",
                oss => "oss",
                pli => "pli",
                pus => "pus",
                fas => "fas",
                pol => "pol",
                por => "por",
                pan => "pan",
                que => "que",
                ron => "ron",
                roh => "roh",
                run => "run",
                rus => "rus",
                sme => "sme",
                smo => "smo",
                sag => "sag",
                san => "san",
                srd => "srd",
                srp => "srp",
                sna => "sna",
                snd => "snd",
                sin => "sin",
                slk => "slk",
                slv => "slv",
                som => "som",
                sot => "sot",
                spa => "spa",
                sun => "sun",
                swa => "swa",
                ssw => "ssw",
                swe => "swe",
                tgl => "tgl",
                tah => "tah",
                tgk => "tgk",
                tam => "tam",
                tat => "tat",
                tel => "tel",
                tha => "tha",
                bod => "bod",
                tir => "tir",
                ton => "ton",
                tso => "tso",
                tsn => "tsn",
                tur => "tur",
                tuk => "tuk",
                twi => "twi",
                uig => "uig",
                ukr => "ukr",
                urd => "urd",
                uzb => "uzb",
                ven => "ven",
                vie => "vie",
                vol => "vol",
                wln => "wln",
                cym => "cym",
                wol => "wol",
                xho => "xho",
                iii => "iii",
                yid => "yid",
                yor => "yor",
                zha => "zha",
                zul => "zul",
                ALL => "ALL",
                _ => "",
            };
        }

        public static LanguageCodes Parse(string name)
        {
            return name switch
            {
                "" => new(0),
                "und" => new(und),

                "abk" => new(abk),
                "ab" => new(ab),
                "Abkhazian" => new(Abkhazian),

                "aar" => new(aar),
                "aa" => new(aa),
                "Afar" => new(Afar),

                "afr" => new(afr),
                "af" => new(af),
                "Afrikaans" => new(Afrikaans),

                "aka" => new(aka),
                "ak" => new(ak),
                "Akan" => new(Akan),

                "sqi" => new(sqi),
                "alb" => new(alb),
                "sq" => new(sq),
                "Albanian" => new(Albanian),

                "amh" => new(amh),
                "am" => new(am),
                "Amharic" => new(Amharic),

                "ara" => new(ara),
                "ar" => new(ar),
                "Arabic" => new(Arabic),

                "arg" => new(arg),
                "an" => new(an),
                "Aragonese" => new(Aragonese),

                "hye" => new(hye),
                "arm" => new(arm),
                "hy" => new(hy),
                "Armenian" => new(Armenian),

                "asm" => new(asm),
                "as" => new(@as),
                "Assamese" => new(Assamese),

                "ava" => new(ava),
                "av" => new(av),
                "Avaric" => new(Avaric),

                "ave" => new(ave),
                "ae" => new(ae),
                "Avestan" => new(Avestan),

                "aym" => new(aym),
                "ay" => new(ay),
                "Aymara" => new(Aymara),

                "aze" => new(aze),
                "az" => new(az),
                "Azerbaijani" => new(Azerbaijani),

                "bam" => new(bam),
                "bm" => new(bm),
                "Bambara" => new(Bambara),

                "bak" => new(bak),
                "ba" => new(ba),
                "Bashkir" => new(Bashkir),

                "eus" => new(eus),
                "baq" => new(baq),
                "eu" => new(eu),
                "Basque" => new(Basque),

                "bel" => new(bel),
                "be" => new(be),
                "Belarusian" => new(Belarusian),

                "ben" => new(ben),
                "bn" => new(bn),
                "Bengali" => new(Bengali),

                "bis" => new(bis),
                "bi" => new(bi),
                "Bislama" => new(Bislama),

                "bos" => new(bos),
                "bs" => new(bs),
                "Bosnian" => new(Bosnian),

                "bre" => new(bre),
                "br" => new(br),
                "Breton" => new(Breton),

                "bul" => new(bul),
                "bg" => new(bg),
                "Bulgarian" => new(Bulgarian),

                "mya" => new(mya),
                "bur" => new(bur),
                "my" => new(my),
                "Burmese" => new(Burmese),

                "cat" => new(cat),
                "ca" => new(ca),
                "Catalan" => new(Catalan),

                "cha" => new(cha),
                "ch" => new(ch),
                "Chamorro" => new(Chamorro),

                "che" => new(che),
                "ce" => new(ce),
                "Chechen" => new(Chechen),

                "nya" => new(nya),
                "ny" => new(ny),
                "Chichewa" => new(Chichewa),

                "zho" => new(zho),
                "chi" => new(chi),
                "zh" => new(zh),
                "Chinese" => new(Chinese),

                "chu" => new(chu),
                "cu" => new(cu),
                "Old_Slavonic" => new(Old_Slavonic),

                "chv" => new(chv),
                "cv" => new(cv),
                "Chuvash" => new(Chuvash),

                "cor" => new(cor),
                "kw" => new(kw),
                "Cornish" => new(Cornish),

                "cos" => new(cos),
                "co" => new(co),
                "Corsican" => new(Corsican),

                "cre" => new(cre),
                "cr" => new(cr),
                "Cree" => new(Cree),

                "hrv" => new(hrv),
                "hr" => new(hr),
                "Croatian" => new(Croatian),

                "ces" => new(ces),
                "cze" => new(cze),
                "cs" => new(cs),
                "Czech" => new(Czech),

                "dan" => new(dan),
                "da" => new(da),
                "Danish" => new(Danish),

                "div" => new(div),
                "dv" => new(dv),
                "Divehi" => new(Divehi),

                "nld" => new(nld),
                "dut" => new(dut),
                "nl" => new(nl),
                "Dutch" => new(Dutch),

                "dzo" => new(dzo),
                "dz" => new(dz),
                "Dzongkha" => new(Dzongkha),

                "eng" => new(eng),
                "en" => new(en),
                "English" => new(English),

                "epo" => new(epo),
                "eo" => new(eo),
                "Esperanto" => new(Esperanto),

                "est" => new(est),
                "et" => new(et),
                "Estonian" => new(Estonian),

                "ewe" => new(ewe),
                "ee" => new(ee),

                "fao" => new(fao),
                "fo" => new(fo),
                "Faroese" => new(Faroese),

                "fij" => new(fij),
                "fj" => new(fj),
                "Fijian" => new(Fijian),

                "fin" => new(fin),
                "fi" => new(fi),
                "Finnish" => new(Finnish),

                "fra" => new(fra),
                "fre" => new(fre),
                "fr" => new(fr),
                "French" => new(French),

                "fry" => new(fry),
                "fy" => new(fy),
                "Western_Frisian" => new(Western_Frisian),

                "ful" => new(ful),
                "ff" => new(ff),
                "Fulah" => new(Fulah),

                "gla" => new(gla),
                "gd" => new(gd),
                "Gaelic" => new(Gaelic),

                "glg" => new(glg),
                "gl" => new(gl),
                "Galician" => new(Galician),

                "lug" => new(lug),
                "lg" => new(lg),
                "Ganda" => new(Ganda),

                "kat" => new(kat),
                "geo" => new(geo),
                "ka" => new(ka),
                "Georgian" => new(Georgian),

                "deu" => new(deu),
                "ger" => new(ger),
                "de" => new(de),
                "German" => new(German),

                "ell" => new(ell),
                "gre" => new(gre),
                "el" => new(el),
                "Greek" => new(Greek),

                "kal" => new(kal),
                "kl" => new(kl),
                "Greenlandic" => new(Greenlandic),

                "grn" => new(grn),
                "gn" => new(gn),
                "Guarani" => new(Guarani),

                "guj" => new(guj),
                "gu" => new(gu),
                "Gujarati" => new(Gujarati),

                "hat" => new(hat),
                "ht" => new(ht),
                "Haitian_Creole" => new(Haitian_Creole),

                "hau" => new(hau),
                "ha" => new(ha),
                "Hausa" => new(Hausa),

                "heb" => new(heb),
                "he" => new(he),
                "Hebrew" => new(Hebrew),

                "her" => new(her),
                "hz" => new(hz),
                "Herero" => new(Herero),

                "hin" => new(hin),
                "hi" => new(hi),
                "Hindi" => new(Hindi),

                "hmo" => new(hmo),
                "ho" => new(ho),
                "Hiri_Motu" => new(Hiri_Motu),

                "hun" => new(hun),
                "hu" => new(hu),
                "Hungarian" => new(Hungarian),

                "isl" => new(isl),
                "ice" => new(ice),
                "is" => new(@is),
                "Icelandic" => new(Icelandic),

                "ido" => new(ido),
                "io" => new(io),
                "Ido" => new(Ido),

                "ibo" => new(ibo),
                "ig" => new(ig),
                "Igbo" => new(Igbo),

                "ind" => new(ind),
                "id" => new(id),
                "Indonesian" => new(Indonesian),

                "ina" => new(ina),
                "ia" => new(ia),
                "Interlingua" => new(Interlingua),

                "ile" => new(ile),
                "ie" => new(ie),
                "Interlingue_Occidental" => new(Interlingue_Occidental),

                "iku" => new(iku),
                "iu" => new(iu),
                "Inuktitut" => new(Inuktitut),

                "ipk" => new(ipk),
                "ik" => new(ik),
                "Inupiaq" => new(Inupiaq),

                "gle" => new(gle),
                "ga" => new(ga),
                "Irish" => new(Irish),

                "ita" => new(ita),
                "it" => new(it),
                "Italian" => new(Italian),

                "jpn" => new(jpn),
                "ja" => new(ja),
                "Japanese" => new(Japanese),

                "jav" => new(jav),
                "jv" => new(jv),
                "Javanese" => new(Javanese),

                "kan" => new(kan),
                "kn" => new(kn),
                "Kannada" => new(Kannada),

                "kau" => new(kau),
                "kr" => new(kr),
                "Kanuri" => new(Kanuri),

                "kas" => new(kas),
                "ks" => new(ks),
                "Kashmiri" => new(Kashmiri),

                "kaz" => new(kaz),
                "kk" => new(kk),
                "Kazakh" => new(Kazakh),

                "khm" => new(khm),
                "km" => new(km),
                "Central_Khmer" => new(Central_Khmer),

                "kik" => new(kik),
                "ki" => new(ki),
                "Kikuyu" => new(Kikuyu),

                "kin" => new(kin),
                "rw" => new(rw),
                "Kinyarwanda" => new(Kinyarwanda),

                "kir" => new(kir),
                "ky" => new(ky),
                "Kirghiz" => new(Kirghiz),

                "kom" => new(kom),
                "kv" => new(kv),
                "Komi" => new(Komi),

                "kon" => new(kon),
                "kg" => new(kg),
                "Kongo" => new(Kongo),

                "kor" => new(kor),
                "ko" => new(ko),
                "Korean" => new(Korean),

                "kua" => new(kua),
                "kj" => new(kj),
                "Kuanyama" => new(Kuanyama),

                "kur" => new(kur),
                "ku" => new(ku),
                "Kurdish" => new(Kurdish),

                "lao" => new(lao),
                "lo" => new(lo),
                "Lao" => new(Lao),

                "lat" => new(lat),
                "la" => new(la),
                "Latin" => new(Latin),

                "lav" => new(lav),
                "lv" => new(lv),
                "Latvian" => new(Latvian),

                "lim" => new(lim),
                "li" => new(li),
                "Limburgan" => new(Limburgan),

                "lin" => new(lin),
                "ln" => new(ln),
                "Lingala" => new(Lingala),

                "lit" => new(lit),
                "lt" => new(lt),
                "Lithuanian" => new(Lithuanian),

                "lub" => new(lub),
                "lu" => new(lu),
                "Luba_Katanga" => new(Luba_Katanga),

                "ltz" => new(ltz),
                "lb" => new(lb),
                "Luxembourgish" => new(Luxembourgish),

                "mkd" => new(mkd),
                "mac" => new(mac),
                "mk" => new(mk),
                "Macedonian" => new(Macedonian),

                "mlg" => new(mlg),
                "mg" => new(mg),
                "Malagasy" => new(Malagasy),

                "msa" => new(msa),
                "may" => new(may),
                "ms" => new(ms),
                "Malay" => new(Malay),

                "mal" => new(mal),
                "ml" => new(ml),
                "Malayalam" => new(Malayalam),

                "mlt" => new(mlt),
                "mt" => new(mt),
                "Maltese" => new(Maltese),

                "glv" => new(glv),
                "gv" => new(gv),
                "Manx" => new(Manx),

                "mri" => new(mri),
                "mao" => new(mao),
                "mi" => new(mi),
                "Maori" => new(Maori),

                "mar" => new(mar),
                "mr" => new(mr),
                "Marathi" => new(Marathi),

                "mah" => new(mah),
                "mh" => new(mh),
                "Marshallese" => new(Marshallese),

                "mon" => new(mon),
                "mn" => new(mn),
                "Mongolian" => new(Mongolian),

                "nau" => new(nau),
                "na" => new(na),
                "Nauru" => new(Nauru),

                "nav" => new(nav),
                "nv" => new(nv),
                "Navajo" => new(Navajo),

                "nde" => new(nde),
                "nd" => new(nd),
                "North_Ndebele" => new(North_Ndebele),

                "nbl" => new(nbl),
                "nr" => new(nr),
                "South_Ndebele" => new(South_Ndebele),

                "ndo" => new(ndo),
                "ng" => new(ng),
                "Ndonga" => new(Ndonga),

                "nep" => new(nep),
                "ne" => new(ne),
                "Nepali" => new(Nepali),

                "nor" => new(nor),
                "no" => new(no),
                "Norwegian" => new(Norwegian),

                "nob" => new(nob),
                "nb" => new(nb),
                "Norwegian_Bokmal" => new(Norwegian_Bokmal),

                "nno" => new(nno),
                "nn" => new(nn),
                "Norwegian_Nynorsk" => new(Norwegian_Nynorsk),

                "oci" => new(oci),
                "oc" => new(oc),
                "Occitan" => new(Occitan),

                "oji" => new(oji),
                "oj" => new(oj),
                "Ojibwa" => new(Ojibwa),

                "ori" => new(ori),
                "or" => new(or),
                "Oriya" => new(Oriya),

                "orm" => new(orm),
                "om" => new(om),
                "Oromo" => new(Oromo),

                "oss" => new(oss),
                "os" => new(os),
                "Ossetian" => new(Ossetian),

                "pli" => new(pli),
                "pi" => new(pi),
                "Pali" => new(Pali),

                "pus" => new(pus),
                "ps" => new(ps),
                "Pashto" => new(Pashto),

                "fas" => new(fas),
                "per" => new(per),
                "fa" => new(fa),
                "Persian" => new(Persian),

                "pol" => new(pol),
                "pl" => new(pl),
                "Polish" => new(Polish),

                "por" => new(por),
                "pt" => new(pt),
                "Portuguese" => new(Portuguese),

                "pan" => new(pan),
                "pa" => new(pa),
                "Punjabi" => new(Punjabi),

                "que" => new(que),
                "qu" => new(qu),
                "Quechua" => new(Quechua),

                "ron" => new(ron),
                "rum" => new(rum),
                "ro" => new(ro),
                "Romanian" => new(Romanian),

                "roh" => new(roh),
                "rm" => new(rm),
                "Romansh" => new(Romansh),

                "run" => new(run),
                "rn" => new(rn),
                "Rundi" => new(Rundi),

                "rus" => new(rus),
                "ru" => new(ru),
                "Russian" => new(Russian),

                "sme" => new(sme),
                "se" => new(se),
                "Northern_Sami" => new(Northern_Sami),

                "smo" => new(smo),
                "sm" => new(sm),
                "Samoan" => new(Samoan),

                "sag" => new(sag),
                "sg" => new(sg),
                "Sango" => new(Sango),

                "san" => new(san),
                "sa" => new(sa),
                "Sanskrit" => new(Sanskrit),

                "srd" => new(srd),
                "sc" => new(sc),
                "Sardinian" => new(Sardinian),

                "srp" => new(srp),
                "sr" => new(sr),
                "Serbian" => new(Serbian),

                "sna" => new(sna),
                "sn" => new(sn),
                "Shona" => new(Shona),

                "snd" => new(snd),
                "sd" => new(sd),
                "Sindhi" => new(Sindhi),

                "sin" => new(sin),
                "si" => new(si),
                "Sinhala" => new(Sinhala),

                "slk" => new(slk),
                "slo" => new(slo),
                "sk" => new(sk),
                "Slovak" => new(Slovak),

                "slv" => new(slv),
                "sl" => new(sl),
                "Slovenian" => new(Slovenian),

                "som" => new(som),
                "so" => new(so),
                "Somali" => new(Somali),

                "sot" => new(sot),
                "st" => new(st),
                "Southern_Sotho" => new(Southern_Sotho),

                "spa" => new(spa),
                "es" => new(es),
                "Spanish" => new(Spanish),

                "sun" => new(sun),
                "su" => new(su),
                "Sundanese" => new(Sundanese),

                "swa" => new(swa),
                "sw" => new(sw),
                "Swahili" => new(Swahili),

                "ssw" => new(ssw),
                "ss" => new(ss),
                "Swati" => new(Swati),

                "swe" => new(swe),
                "sv" => new(sv),
                "Swedish" => new(Swedish),

                "tgl" => new(tgl),
                "tl" => new(tl),
                "Tagalog" => new(Tagalog),

                "tah" => new(tah),
                "ty" => new(ty),
                "Tahitian" => new(Tahitian),

                "tgk" => new(tgk),
                "tg" => new(tg),
                "Tajik" => new(Tajik),

                "tam" => new(tam),
                "ta" => new(ta),
                "Tamil" => new(Tamil),

                "tat" => new(tat),
                "tt" => new(tt),
                "Tatar" => new(Tatar),

                "tel" => new(tel),
                "te" => new(te),
                "Telugu" => new(Telugu),

                "tha" => new(tha),
                "th" => new(th),
                "Thai" => new(Thai),

                "bod" => new(bod),
                "tib" => new(tib),
                "bo" => new(bo),
                "Tibetan" => new(Tibetan),

                "tir" => new(tir),
                "ti" => new(ti),
                "Tigrinya" => new(Tigrinya),

                "ton" => new(ton),
                "to" => new(to),
                "Tonga" => new(Tonga),

                "tso" => new(tso),
                "ts" => new(ts),
                "Tsonga" => new(Tsonga),

                "tsn" => new(tsn),
                "tn" => new(tn),
                "Tswana" => new(Tswana),

                "tur" => new(tur),
                "tr" => new(tr),
                "Turkish" => new(Turkish),

                "tuk" => new(tuk),
                "tk" => new(tk),
                "Turkmen" => new(Turkmen),

                "twi" => new(twi),
                "tw" => new(tw),
                "Twi" => new(Twi),

                "uig" => new(uig),
                "ug" => new(ug),
                "Uighur" => new(Uighur),

                "ukr" => new(ukr),
                "uk" => new(uk),
                "Ukrainian" => new(Ukrainian),

                "urd" => new(urd),
                "ur" => new(ur),
                "Urdu" => new(Urdu),

                "uzb" => new(uzb),
                "uz" => new(uz),
                "Uzbek" => new(Uzbek),

                "ven" => new(ven),
                "ve" => new(ve),
                "Venda" => new(Venda),

                "vie" => new(vie),
                "vi" => new(vi),
                "Vietnamese" => new(Vietnamese),

                "vol" => new(vol),
                "vo" => new(vo),
                "Volapük" => new(Volapük),

                "wln" => new(wln),
                "wa" => new(wa),
                "Walloon" => new(Walloon),

                "cym" => new(cym),
                "wel" => new(wel),
                "cy" => new(cy),
                "Welsh" => new(Welsh),

                "wol" => new(wol),
                "wo" => new(wo),
                "Wolof" => new(Wolof),

                "xho" => new(xho),
                "xh" => new(xh),
                "Xhosa" => new(Xhosa),

                "iii" => new(iii),
                "ii" => new(ii),
                "Sichuan_Yi" => new(Sichuan_Yi),

                "yid" => new(yid),
                "yi" => new(yi),
                "Yiddish" => new(Yiddish),

                "yor" => new(yor),
                "yo" => new(yo),
                "Yoruba" => new(Yoruba),

                "zha" => new(zha),
                "za" => new(za),
                "Zhuang" => new(Zhuang),

                "zul" => new(zul),
                "zu" => new(zu),
                "Zulu" => new(Zulu),

                "ALL" => new(ALL),
                _ => new(0),
            };
        }

        public static bool TryParse(string name, out LanguageCodes code)
        {
            code = Parse(name);
            return code != 0;
        }

        public static implicit operator LanguageCodes(int number)
        {
            return new(number);
        }

        #region constants

        public const int ALL = 1000;
        public const int und = 1;

        public const int abk = 2;
        public const int ab = 2;
        public const int Abkhazian = 2;

        public const int aar = 3;
        public const int aa = 3;
        public const int Afar = 3;

        public const int afr = 4;
        public const int af = 4;
        public const int Afrikaans = 4;

        public const int aka = 5;
        public const int ak = 5;
        public const int Akan = 5;

        public const int sqi = 6;
        public const int alb = 6;
        public const int sq = 6;
        public const int Albanian = 6;

        public const int amh = 7;
        public const int am = 7;
        public const int Amharic = 7;

        public const int ara = 8;
        public const int ar = 8;
        public const int Arabic = 8;

        public const int arg = 9;
        public const int an = 9;
        public const int Aragonese = 9;

        public const int hye = 10;
        public const int arm = 10;
        public const int hy = 10;
        public const int Armenian = 10;

        public const int asm = 11;
        public const int @as = 11;
        public const int Assamese = 11;

        public const int ava = 12;
        public const int av = 12;
        public const int Avaric = 12;

        public const int ave = 13;
        public const int ae = 13;
        public const int Avestan = 13;

        public const int aym = 14;
        public const int ay = 14;
        public const int Aymara = 14;

        public const int aze = 15;
        public const int az = 15;
        public const int Azerbaijani = 15;

        public const int bam = 16;
        public const int bm = 16;
        public const int Bambara = 16;

        public const int bak = 17;
        public const int ba = 17;
        public const int Bashkir = 17;

        public const int eus = 18;
        public const int baq = 18;
        public const int eu = 18;
        public const int Basque = 18;

        public const int bel = 19;
        public const int be = 19;
        public const int Belarusian = 19;

        public const int ben = 20;
        public const int bn = 20;
        public const int Bengali = 20;

        public const int bis = 21;
        public const int bi = 21;
        public const int Bislama = 21;

        public const int bos = 22;
        public const int bs = 22;
        public const int Bosnian = 22;

        public const int bre = 23;
        public const int br = 23;
        public const int Breton = 23;

        public const int bul = 24;
        public const int bg = 24;
        public const int Bulgarian = 24;

        public const int mya = 25;
        public const int bur = 25;
        public const int my = 25;
        public const int Burmese = 25;

        public const int cat = 26;
        public const int ca = 26;
        public const int Catalan = 26;

        public const int cha = 27;
        public const int ch = 27;
        public const int Chamorro = 27;

        public const int che = 28;
        public const int ce = 28;
        public const int Chechen = 28;

        public const int nya = 29;
        public const int ny = 29;
        public const int Chichewa = 29;

        public const int zho = 30;
        public const int chi = 30;
        public const int zh = 30;
        public const int Chinese = 30;

        public const int chu = 31;
        public const int cu = 31;
        public const int Old_Slavonic = 31;

        public const int chv = 32;
        public const int cv = 32;
        public const int Chuvash = 32;

        public const int cor = 33;
        public const int kw = 33;
        public const int Cornish = 33;

        public const int cos = 34;
        public const int co = 34;
        public const int Corsican = 34;

        public const int cre = 35;
        public const int cr = 35;
        public const int Cree = 35;

        public const int hrv = 36;
        public const int hr = 36;
        public const int Croatian = 36;

        public const int ces = 37;
        public const int cze = 37;
        public const int cs = 37;
        public const int Czech = 37;

        public const int dan = 38;
        public const int da = 38;
        public const int Danish = 38;

        public const int div = 39;
        public const int dv = 39;
        public const int Divehi = 39;

        public const int nld = 40;
        public const int dut = 40;
        public const int nl = 40;
        public const int Dutch = 40;

        public const int dzo = 41;
        public const int dz = 41;
        public const int Dzongkha = 41;

        public const int eng = 42;
        public const int en = 42;
        public const int English = 42;

        public const int epo = 43;
        public const int eo = 43;
        public const int Esperanto = 43;

        public const int est = 44;
        public const int et = 44;
        public const int Estonian = 44;

        public const int ewe = 45;
        public const int ee = 45;

        public const int fao = 46;
        public const int fo = 46;
        public const int Faroese = 46;

        public const int fij = 47;
        public const int fj = 47;
        public const int Fijian = 47;

        public const int fin = 48;
        public const int fi = 48;
        public const int Finnish = 48;

        public const int fra = 49;
        public const int fre = 49;
        public const int fr = 49;
        public const int French = 49;

        public const int fry = 50;
        public const int fy = 50;
        public const int Western_Frisian = 50;

        public const int ful = 51;
        public const int ff = 51;
        public const int Fulah = 51;

        public const int gla = 52;
        public const int gd = 52;
        public const int Gaelic = 52;

        public const int glg = 53;
        public const int gl = 53;
        public const int Galician = 53;

        public const int lug = 54;
        public const int lg = 54;
        public const int Ganda = 54;

        public const int kat = 55;
        public const int geo = 55;
        public const int ka = 55;
        public const int Georgian = 55;

        public const int deu = 56;
        public const int ger = 56;
        public const int de = 56;
        public const int German = 56;

        public const int ell = 57;
        public const int gre = 57;
        public const int el = 57;
        public const int Greek = 57;

        public const int kal = 58;
        public const int kl = 58;
        public const int Greenlandic = 58;

        public const int grn = 59;
        public const int gn = 59;
        public const int Guarani = 59;

        public const int guj = 60;
        public const int gu = 60;
        public const int Gujarati = 60;

        public const int hat = 61;
        public const int ht = 61;
        public const int Haitian_Creole = 61;

        public const int hau = 62;
        public const int ha = 62;
        public const int Hausa = 62;

        public const int heb = 63;
        public const int he = 63;
        public const int Hebrew = 63;

        public const int her = 64;
        public const int hz = 64;
        public const int Herero = 64;

        public const int hin = 65;
        public const int hi = 65;
        public const int Hindi = 65;

        public const int hmo = 66;
        public const int ho = 66;
        public const int Hiri_Motu = 66;

        public const int hun = 67;
        public const int hu = 67;
        public const int Hungarian = 67;

        public const int isl = 68;
        public const int ice = 68;
        public const int @is = 68;
        public const int Icelandic = 68;

        public const int ido = 69;
        public const int io = 69;
        public const int Ido = 69;

        public const int ibo = 70;
        public const int ig = 70;
        public const int Igbo = 70;

        public const int ind = 71;
        public const int id = 71;
        public const int Indonesian = 71;

        public const int ina = 72;
        public const int ia = 72;
        public const int Interlingua = 72;

        public const int ile = 73;
        public const int ie = 73;
        public const int Interlingue_Occidental = 73;

        public const int iku = 74;
        public const int iu = 74;
        public const int Inuktitut = 74;

        public const int ipk = 75;
        public const int ik = 75;
        public const int Inupiaq = 75;

        public const int gle = 76;
        public const int ga = 76;
        public const int Irish = 76;

        public const int ita = 77;
        public const int it = 77;
        public const int Italian = 77;

        public const int jpn = 78;
        public const int ja = 78;
        public const int Japanese = 78;

        public const int jav = 79;
        public const int jv = 79;
        public const int Javanese = 79;

        public const int kan = 80;
        public const int kn = 80;
        public const int Kannada = 80;

        public const int kau = 81;
        public const int kr = 81;
        public const int Kanuri = 81;

        public const int kas = 82;
        public const int ks = 82;
        public const int Kashmiri = 82;

        public const int kaz = 83;
        public const int kk = 83;
        public const int Kazakh = 83;

        public const int khm = 84;
        public const int km = 84;
        public const int Central_Khmer = 84;

        public const int kik = 85;
        public const int ki = 85;
        public const int Kikuyu = 85;

        public const int kin = 86;
        public const int rw = 86;
        public const int Kinyarwanda = 86;

        public const int kir = 87;
        public const int ky = 87;
        public const int Kirghiz = 87;

        public const int kom = 88;
        public const int kv = 88;
        public const int Komi = 88;

        public const int kon = 89;
        public const int kg = 89;
        public const int Kongo = 89;

        public const int kor = 90;
        public const int ko = 90;
        public const int Korean = 90;

        public const int kua = 91;
        public const int kj = 91;
        public const int Kuanyama = 91;

        public const int kur = 92;
        public const int ku = 92;
        public const int Kurdish = 92;

        public const int lao = 93;
        public const int lo = 93;
        public const int Lao = 93;

        public const int lat = 94;
        public const int la = 94;
        public const int Latin = 94;

        public const int lav = 95;
        public const int lv = 95;
        public const int Latvian = 95;

        public const int lim = 96;
        public const int li = 96;
        public const int Limburgan = 96;

        public const int lin = 97;
        public const int ln = 97;
        public const int Lingala = 97;

        public const int lit = 98;
        public const int lt = 98;
        public const int Lithuanian = 98;

        public const int lub = 99;
        public const int lu = 99;
        public const int Luba_Katanga = 99;

        public const int ltz = 100;
        public const int lb = 100;
        public const int Luxembourgish = 100;

        public const int mkd = 101;
        public const int mac = 101;
        public const int mk = 101;
        public const int Macedonian = 101;

        public const int mlg = 102;
        public const int mg = 102;
        public const int Malagasy = 102;

        public const int msa = 103;
        public const int may = 103;
        public const int ms = 103;
        public const int Malay = 103;

        public const int mal = 104;
        public const int ml = 104;
        public const int Malayalam = 104;

        public const int mlt = 105;
        public const int mt = 105;
        public const int Maltese = 105;

        public const int glv = 106;
        public const int gv = 106;
        public const int Manx = 106;

        public const int mri = 107;
        public const int mao = 107;
        public const int mi = 107;
        public const int Maori = 107;

        public const int mar = 108;
        public const int mr = 108;
        public const int Marathi = 108;

        public const int mah = 109;
        public const int mh = 109;
        public const int Marshallese = 109;

        public const int mon = 110;
        public const int mn = 110;
        public const int Mongolian = 110;

        public const int nau = 111;
        public const int na = 111;
        public const int Nauru = 111;

        public const int nav = 112;
        public const int nv = 112;
        public const int Navajo = 112;

        public const int nde = 113;
        public const int nd = 113;
        public const int North_Ndebele = 113;

        public const int nbl = 114;
        public const int nr = 114;
        public const int South_Ndebele = 114;

        public const int ndo = 115;
        public const int ng = 115;
        public const int Ndonga = 115;

        public const int nep = 116;
        public const int ne = 116;
        public const int Nepali = 116;

        public const int nor = 117;
        public const int no = 117;
        public const int Norwegian = 117;

        public const int nob = 118;
        public const int nb = 118;
        public const int Norwegian_Bokmal = 118;

        public const int nno = 119;
        public const int nn = 119;
        public const int Norwegian_Nynorsk = 119;

        public const int oci = 120;
        public const int oc = 120;
        public const int Occitan = 120;

        public const int oji = 121;
        public const int oj = 121;
        public const int Ojibwa = 121;

        public const int ori = 122;
        public const int or = 122;
        public const int Oriya = 122;

        public const int orm = 123;
        public const int om = 123;
        public const int Oromo = 123;

        public const int oss = 124;
        public const int os = 124;
        public const int Ossetian = 124;

        public const int pli = 125;
        public const int pi = 125;
        public const int Pali = 125;

        public const int pus = 126;
        public const int ps = 126;
        public const int Pashto = 126;

        public const int fas = 127;
        public const int per = 127;
        public const int fa = 127;
        public const int Persian = 127;

        public const int pol = 128;
        public const int pl = 128;
        public const int Polish = 128;

        public const int por = 129;
        public const int pt = 129;
        public const int Portuguese = 129;

        public const int pan = 130;
        public const int pa = 130;
        public const int Punjabi = 130;

        public const int que = 131;
        public const int qu = 131;
        public const int Quechua = 131;

        public const int ron = 132;
        public const int rum = 132;
        public const int ro = 132;
        public const int Romanian = 132;

        public const int roh = 133;
        public const int rm = 133;
        public const int Romansh = 133;

        public const int run = 134;
        public const int rn = 134;
        public const int Rundi = 134;

        public const int rus = 135;
        public const int ru = 135;
        public const int Russian = 135;

        public const int sme = 136;
        public const int se = 136;
        public const int Northern_Sami = 136;

        public const int smo = 137;
        public const int sm = 137;
        public const int Samoan = 137;

        public const int sag = 138;
        public const int sg = 138;
        public const int Sango = 138;

        public const int san = 139;
        public const int sa = 139;
        public const int Sanskrit = 139;

        public const int srd = 140;
        public const int sc = 140;
        public const int Sardinian = 140;

        public const int srp = 141;
        public const int sr = 141;
        public const int Serbian = 141;

        public const int sna = 142;
        public const int sn = 142;
        public const int Shona = 142;

        public const int snd = 143;
        public const int sd = 143;
        public const int Sindhi = 143;

        public const int sin = 144;
        public const int si = 144;
        public const int Sinhala = 144;

        public const int slk = 145;
        public const int slo = 145;
        public const int sk = 145;
        public const int Slovak = 145;

        public const int slv = 146;
        public const int sl = 146;
        public const int Slovenian = 146;

        public const int som = 147;
        public const int so = 147;
        public const int Somali = 147;

        public const int sot = 148;
        public const int st = 148;
        public const int Southern_Sotho = 148;

        public const int spa = 149;
        public const int es = 149;
        public const int Spanish = 149;

        public const int sun = 150;
        public const int su = 150;
        public const int Sundanese = 150;

        public const int swa = 151;
        public const int sw = 151;
        public const int Swahili = 151;

        public const int ssw = 152;
        public const int ss = 152;
        public const int Swati = 152;

        public const int swe = 153;
        public const int sv = 153;
        public const int Swedish = 153;

        public const int tgl = 154;
        public const int tl = 154;
        public const int Tagalog = 154;

        public const int tah = 155;
        public const int ty = 155;
        public const int Tahitian = 155;

        public const int tgk = 156;
        public const int tg = 156;
        public const int Tajik = 156;

        public const int tam = 157;
        public const int ta = 157;
        public const int Tamil = 157;

        public const int tat = 158;
        public const int tt = 158;
        public const int Tatar = 158;

        public const int tel = 159;
        public const int te = 159;
        public const int Telugu = 159;

        public const int tha = 160;
        public const int th = 160;
        public const int Thai = 160;

        public const int bod = 161;
        public const int tib = 161;
        public const int bo = 161;
        public const int Tibetan = 161;

        public const int tir = 162;
        public const int ti = 162;
        public const int Tigrinya = 162;

        public const int ton = 163;
        public const int to = 163;
        public const int Tonga = 163;

        public const int tso = 164;
        public const int ts = 164;
        public const int Tsonga = 164;

        public const int tsn = 165;
        public const int tn = 165;
        public const int Tswana = 165;

        public const int tur = 166;
        public const int tr = 166;
        public const int Turkish = 166;

        public const int tuk = 167;
        public const int tk = 167;
        public const int Turkmen = 167;

        public const int twi = 168;
        public const int tw = 168;
        public const int Twi = 168;

        public const int uig = 169;
        public const int ug = 169;
        public const int Uighur = 169;

        public const int ukr = 170;
        public const int uk = 170;
        public const int Ukrainian = 170;

        public const int urd = 171;
        public const int ur = 171;
        public const int Urdu = 171;

        public const int uzb = 172;
        public const int uz = 172;
        public const int Uzbek = 172;

        public const int ven = 173;
        public const int ve = 173;
        public const int Venda = 173;

        public const int vie = 174;
        public const int vi = 174;
        public const int Vietnamese = 174;

        public const int vol = 175;
        public const int vo = 175;
        public const int Volapük = 175;

        public const int wln = 176;
        public const int wa = 176;
        public const int Walloon = 176;

        public const int cym = 177;
        public const int wel = 177;
        public const int cy = 177;
        public const int Welsh = 177;

        public const int wol = 178;
        public const int wo = 178;
        public const int Wolof = 178;

        public const int xho = 179;
        public const int xh = 179;
        public const int Xhosa = 179;

        public const int iii = 180;
        public const int ii = 180;
        public const int Sichuan_Yi = 180;

        public const int yid = 181;
        public const int yi = 181;
        public const int Yiddish = 181;

        public const int yor = 182;
        public const int yo = 182;
        public const int Yoruba = 182;

        public const int zha = 183;
        public const int za = 183;
        public const int Zhuang = 183;

        public const int zul = 184;
        public const int zu = 184;
        public const int Zulu = 184;

        #endregion

        #region converter

        public class Converter : JsonConverter<LanguageCodes>
        {
            public override LanguageCodes Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                    return LanguageCodes.Parse(reader.GetString());
                if (reader.TokenType == JsonTokenType.Number)
                    return (LanguageCodes)reader.GetInt32();
                return (LanguageCodes)0;
            }

            public override void Write(Utf8JsonWriter writer, LanguageCodes value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

        #endregion

        #region equality

        public static bool operator ==(LanguageCodes value1, LanguageCodes value2)
        {
            return value1.Int == value2.Int;
        }

        public static bool operator !=(LanguageCodes value1, LanguageCodes value2)
        {
            return value1.Int != value2.Int;
        }

        public static bool operator ==(LanguageCodes value1, int value2)
        {
            return value1.Int == value2;
        }

        public static bool operator !=(LanguageCodes value1, int value2)
        {
            return value1.Int != value2;
        }

        public readonly override bool Equals(object obj)
        {
            if (obj is LanguageCodes code)
                return this.Int == code.Int;
            if (obj is int number)
                return this.Int == number;
            if (obj is string name)
                return this == LanguageCodes.Parse(name);
            return false;
        }

        public readonly override int GetHashCode()
        {
            return this.Int.GetHashCode();
        }

        #endregion
    }
}