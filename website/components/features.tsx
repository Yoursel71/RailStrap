import {
  PlayCircle,
  Wifi,
  RotateCcw,
  SlidersHorizontal,
  Package,
  Palette,
  Users,
  MessageCircle,
  BarChart3,
  FileText,
} from "lucide-react";
import { Card } from "@/components/ui/card";

const features = [
  {
    icon: PlayCircle,
    title: "Custom loading screen",
    desc: "A video splash plays while Roblox boots instead of a blank window — bring your own, muted and looped automatically.",
    span: "sm:col-span-3",
  },
  {
    icon: Wifi,
    title: "Ping overlay & server hop",
    desc: "Live connection ping shown in-game, plus reroll into a fresh server instantly without leaving and rejoining by hand.",
    span: "sm:col-span-3",
  },
  {
    icon: RotateCcw,
    title: "Auto-restart on crash",
    desc: "If Roblox crashes unexpectedly, RailStrap relaunches it for you.",
    span: "sm:col-span-2",
  },
  {
    icon: SlidersHorizontal,
    title: "FastFlag presets",
    desc: "One-click toggles for uncapped FPS, MSAA and texture quality — no manual config editing.",
    span: "sm:col-span-2",
  },
  {
    icon: Package,
    title: "Studio plugin manager",
    desc: "Browse, disable and remove installed Roblox Studio plugins from one place.",
    span: "sm:col-span-2",
  },
  {
    icon: Palette,
    title: "Theme & mod gallery",
    desc: "Install community bootstrapper themes and content mods directly, or build your own with the built-in editor.",
    span: "sm:col-span-3",
  },
  {
    icon: Users,
    title: "Friend activity panel",
    desc: "Optional and off by default. See what friends are playing using your own Roblox cookie — encrypted on-device, never uploaded.",
    span: "sm:col-span-3",
  },
  {
    icon: MessageCircle,
    title: "Discord Rich Presence",
    desc: "Let friends see what you're playing at a glance, with server-join support.",
    span: "sm:col-span-2",
  },
  {
    icon: BarChart3,
    title: "Playtime stats",
    desc: "A private, local log of your play sessions per game — nothing leaves your PC.",
    span: "sm:col-span-2",
  },
  {
    icon: FileText,
    title: "Log viewer & import/export",
    desc: "Search Roblox's log output without leaving RailStrap, and back up or move your settings in one click.",
    span: "sm:col-span-2",
  },
];

export function Features() {
  return (
    <section id="features" className="py-24">
      <div className="container">
        <p className="mb-2.5 text-xs font-bold uppercase tracking-wider text-[hsl(217,100%,68%)]">
          Features
        </p>
        <h2 className="mb-3 text-[28px] font-extrabold tracking-tight">
          Everything the default launcher skips
        </h2>
        <p className="mb-11 max-w-[58ch] text-[15px] text-muted-foreground">
          RailStrap sits between Windows and Roblox, adding quality-of-life features without
          touching how the game itself runs — and without a ban risk, since it never interacts
          with the client the way exploits do.
        </p>

        <div className="grid grid-cols-1 gap-3.5 sm:grid-cols-6">
          {features.map((f) => (
            <Card key={f.title} className={f.span}>
              <div className="mb-4 flex h-[38px] w-[38px] items-center justify-center rounded-[10px] border border-[#7c5cff]/20 bg-[#7c5cff]/10 text-[#a998ff]">
                <f.icon className="h-[19px] w-[19px]" strokeWidth={1.7} />
              </div>
              <h3 className="mb-1.5 text-[15px] font-bold tracking-tight">{f.title}</h3>
              <p className="text-[13.5px] leading-relaxed text-muted-foreground">{f.desc}</p>
            </Card>
          ))}
        </div>
      </div>
    </section>
  );
}
