
function getForwardPos(player, distance)
{
    var pos = API.getEntityPosition(player);
    var rot = API.getEntityRotation(player).Z;

    var xOff = -(Math.sin((rot * Math.PI) / 180) * distance);
    var yOff = Math.cos((rot * Math.PI) / 180) * distance;

    return pos.Add(new Vector3(xOff, yOff, 0));
}

function getWeaponClass(hash)
{
    switch (hash)
    {
        case -1786099057:
        case -2067956739:
        case 1141786504:
        case -1951375401:
        case 1317494643:
        case -656458692:
        case 1737195953:
        case -1810795771:
        case 419712736:
            return 8;
        case 911657153:
            return 7;
        case -102323637:
        case -853065399:
        case -1834847097:
        case -102973651:
        case -1716189206:
        case -538741184:
            return 9;
        case 584646201:
        case 453432689:
        case -1076751822:
        case 1593441988:
            return 1;
        case -1045183535:
        case -598887786:
        case -1716589765:
        case -771403250:
        case 137902532:
            return 0;
        case 324215364:
        case -619010992:
        case 736523883:
        case -270015777:
        case 171789620:
        case -1121678507:
        case 1627465347:
            return 2;
        case -1074790547:
        case -2084633992:
        case -1063057011:
        case -1357824103:
        case 2132975508:
        case 1649403952:
            return 3;
        case 100416529:
        case 205991906:
            return 4;
        case -1569615261:
            return 10;
        default:
            return "N/A";
    }
}