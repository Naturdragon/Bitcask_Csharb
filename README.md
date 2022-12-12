# Bitcask_Csharb
## !Work is discontiniute!

## Wichtige Begriffe
Append:
    Append bedeutet das man nur unten dazu schreibt. ZB. Bei einer Tabelle wird nur ganz unten immer dazu geschrieben.
    Beim Umdaten wird auch nur hinzugefügt. Dadurch hat man dann die veralteteb Daten und die aktuellen Datein im Selben File. Das ist aber kein Problem, da man von         unten zum Lesen beginnt. Das heist das man die Aktuelle Datei zuerst findet und dann einfach nicht mehr weiter sucht.
    Beim Löschen updatet man den zu löschenden Datensatz mit einem Tomb Stone (Grabstein). Ein Tomb Stone zeigt das dieser Wert gelöscht ist. Der Tomb Stone sollte ein       value haben, welches zu keinen Problemen führt, wenn man damit gelöschte daten Makiert. Ein nachteil an dem Tomb Stone ist allerdings, das es zwar bekannt ist, das       die Daten gelöscht sind, aber das bedeutet das die Werte noch immer in File drinnen sind.
