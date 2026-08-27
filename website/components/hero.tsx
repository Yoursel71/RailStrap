"use client";

import Image from "next/image";
import { ArrowDownToLine, Github, ShieldCheck } from "lucide-react";
import { ContainerScroll } from "@/components/ui/container-scroll-animation";
import { Button } from "@/components/ui/button";

export function Hero() {
  return (
    <section id="hero" className="relative overflow-hidden">
      <div className="pointer-events-none absolute -left-40 -top-56 h-[34rem] w-[34rem] rounded-full bg-[#6d3fff]/25 blur-[110px]" />
      <div className="pointer-events-none absolute -right-40 -top-32 h-[30rem] w-[30rem] rounded-full bg-[#3f7bff]/20 blur-[110px]" />

      <ContainerScroll
        titleComponent={
          <div className="flex flex-col items-center px-4">
            <span className="mb-6 inline-flex items-center gap-2 rounded-full border border-[#7c5cff]/30 bg-[#7c5cff]/10 py-1.5 pl-2 pr-3 text-xs font-semibold text-[#c3b6ff] shadow-[0_0_30px_rgba(124,92,255,.12)]">
              <span className="h-1.5 w-1.5 rounded-full bg-[#9c82ff] shadow-[0_0_0_3px_rgba(124,92,255,.25)]" />
              Version 2.13.0 · Free and open source
            </span>

            <h1 className="max-w-[14ch] text-5xl font-extrabold leading-[0.98] tracking-[-0.045em] text-foreground sm:text-6xl md:text-[76px]">
              Your Roblox launcher, {" "}
              <span className="bg-gradient-to-br from-[#b49cff] via-[#846fff] to-[#4f9dff] bg-clip-text text-transparent">
                upgraded.
              </span>
            </h1>

            <p className="mx-auto mt-6 max-w-[56ch] text-base leading-7 text-muted-foreground sm:text-lg">
              Launch faster, tune graphics, manage mods and keep useful stats — all from one
              private, open-source Windows app.
            </p>

            <div className="mt-8 flex w-full flex-col items-center justify-center gap-3 sm:w-auto sm:flex-row">
              <Button size="lg" className="w-full sm:w-auto" asChild>
                <a href="https://github.com/Yoursel71/RailStrap/releases/latest" target="_blank" rel="noopener noreferrer">
                  <ArrowDownToLine /> Download for Windows
                </a>
              </Button>
              <Button size="lg" variant="ghost" className="w-full sm:w-auto" asChild>
                <a href="https://github.com/Yoursel71/RailStrap" target="_blank" rel="noopener noreferrer">
                  <Github /> View source
                </a>
              </Button>
            </div>

            <p className="mt-4 flex items-center justify-center gap-1.5 text-[12.5px] text-zinc-500">
              <ShieldCheck className="h-3.5 w-3.5" />
              Windows 10/11 · .NET 6 · no account required
            </p>
          </div>
        }
      >
        <div className="relative h-full w-full">
          <Image
            src="/screenshots/launch-menu.png"
            alt="RailStrap launch menu showing Roblox, Studio, settings, and help actions"
            fill
            sizes="(min-width: 1024px) 1000px, 90vw"
            className="object-cover"
            priority
            draggable={false}
          />
        </div>
      </ContainerScroll>
    </section>
  );
}
